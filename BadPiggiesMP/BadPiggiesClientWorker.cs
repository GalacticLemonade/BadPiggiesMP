using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace BadPiggiesMP {
    public class BadPiggiesClientWorker {
        private readonly TcpClient socket;
        private readonly IPEndPoint endpoint;
        private readonly BPStream socketStream;

        private static readonly Dictionary<string, Game> Games = new Dictionary<string, Game>();
        
        public BadPiggiesClientWorker(TcpClient socket) {
            this.socket = socket;
            this.endpoint = socket.Client.RemoteEndPoint as IPEndPoint ?? throw new IOException("no endpoint");
            this.socketStream = new BPStream(socket.GetStream());
        }

        public void Start() {
            //create a new thread for the run method and start it
            new Thread(Run).Start();
        }

        //main listen loop
        private void Run() {
            try {
                //listen forever
                while (true) {
                    //do reading in here

                    //read packet
                    string gameId = socketStream.ReadString();
                    PacketTypes.ServerPackets packetId = (PacketTypes.ServerPackets)socketStream.ReadByte();

                    switch (packetId) {
                        case PacketTypes.ServerPackets.REQ_PLACE_PART:
                            //read part pos
                            int partX = socketStream.ReadVarInt();
                            int partY = socketStream.ReadVarInt();
                            //read part type
                            PartType type = (PartType)socketStream.ReadShort();

                            Console.WriteLine($"Place part at {partX}, {partY}, with type {type}");
                            
                            //write data back to clients
                            Games[gameId].GetBroadcastBPStream().WriteByte(PacketTypes.ClientPackets.PLACE_PART);
                            Games[gameId].GetBroadcastBPStream().WriteVarInt(partX); //x
                            Games[gameId].GetBroadcastBPStream().WriteVarInt(partY); //y
                            Games[gameId].GetBroadcastBPStream().WriteShort((short)type); //type
                            break;
                        //request game start
                        case PacketTypes.ServerPackets.REQ_GAME_START:
                            Console.WriteLine("Game start requested!");

                            //tell a random client to send their physics info so we can relay it
                            Random random = new Random();
                            int selectedClient = random.Next(0, Games[gameId].clients.Count);
                            Games[gameId].clients[selectedClient].WriteByte(PacketTypes.ClientPackets.SELECTED);
                            
                            Games[gameId].Start();
                            
                            //write data back to clients
                            Games[gameId].GetBroadcastBPStream().WriteByte(PacketTypes.ClientPackets.GAME_START);
                            break;
                        //request switch level
                        case PacketTypes.ServerPackets.REQ_LEVEL_SWITCH:
                            // read levelid
                            int levelId = socketStream.ReadInt();

                            Console.WriteLine($"Load level {levelId}");
                            
                            //write data to client
                            Games[gameId].GetBroadcastBPStream().WriteByte(PacketTypes.ClientPackets.LEVEL_SWITCH);
                            Games[gameId].GetBroadcastBPStream().WriteInt(levelId);
                            break;
                        //activate part
                        case PacketTypes.ServerPackets.REQ_ACTIVATE_PART:
                            //read part network id
                            int partNetId = socketStream.ReadVarInt();

                            Console.WriteLine($"Client requested us to activate part with NID {partNetId}");
                            
                            //write data back to client
                            Games[gameId].GetBroadcastBPStream().WriteByte(PacketTypes.ClientPackets.ACTIVATE_PART);
                            Games[gameId].GetBroadcastBPStream().WriteInt(partNetId);
                            break;
                        //connect player to game
                        case PacketTypes.ServerPackets.CONNECT:
                            string username = socketStream.ReadString();

                            if (Games.TryGetValue(gameId, out Game game))
                            {
                                Console.WriteLine("Game exists");
                                //add player to game
                                game.AddPlayer(socketStream, username);
                            } else
                            {
                                Console.WriteLine("Creating game");
                                //create new game
                                Game newGame = new Game(gameId);
                                //add it to the game list
                                Games.Add(gameId, newGame);
                                //add player to game
                                newGame.AddPlayer(socketStream, username);
                            }
    
                            Console.WriteLine($"Connecting user with username of {username} and gameid of {gameId}");
                            break;
                        //client telling server physics info
                        case PacketTypes.ServerPackets.PART_UPDATE:

                            int numParts = socketStream.ReadVarInt();
                            int netId = socketStream.ReadVarInt();
                            Vector2 position = socketStream.ReadVec2();
                            float rotZ = socketStream.ReadFloat();
                            float rotW = socketStream.ReadFloat();
                            
                            //relay all that data back to every client i think or smth
                            Games[gameId].GetBroadcastBPStream().WriteByte(PacketTypes.ClientPackets.PART_UPDATE);
                            Games[gameId].GetBroadcastBPStream().WriteVarInt(numParts);
                            Games[gameId].GetBroadcastBPStream().WriteVarInt(netId);
                            Games[gameId].GetBroadcastBPStream().WriteVec2(position);
                            Games[gameId].GetBroadcastBPStream().WriteFloat(rotZ);
                            Games[gameId].GetBroadcastBPStream().WriteFloat(rotW);
                            
                            break;
                        default:
                            Console.WriteLine($"Unknown packet {packetId}!");

                            //send kick packet
                            socketStream.WriteByte(PacketTypes.ClientPackets.KICK);
                            socketStream.WriteString("Invalid packet");

                            break;
                    }

                    Console.WriteLine($"{endpoint.Port} sent packet with game id {gameId}, and packet id {packetId}!");
                }
            }
            //ignore exceptions to not bring down the entire server on one
            catch (Exception ex) {
                //log the exception
                Console.WriteLine(ex);

                //close the connection on exception
                socket.Close();
                socketStream.Dispose();
            }
        }
    }
}