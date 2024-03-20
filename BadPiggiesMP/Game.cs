using System.Collections.Generic;

namespace BadPiggiesMP
{
    public class Game
    {
        public readonly List<BPStream> clients;
        private readonly BPStream clientsStream;
        private readonly GameStream clientGameStream;
        private readonly string gameId;
        //public readonly BPWorld World;

        /// <summary>
        /// Stream that writes to all clients at once.
        /// </summary>
        public class GameStream : Stream
        {
            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            //list of clients
            public readonly List<BPStream> clients;

            public GameStream(List<BPStream> clientList)
            {
                this.clients = clientList;
            }

            public override void Flush()
            {
                //throw new NotImplementedException();
                
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                //write to all clients
                for (int i = 0; i < clients.Count; i++)
                {
                    clients[i].WriteBytes(buffer, offset, count);
                }
            }
        }

        public Game(string id)
        {
            this.clients = new List<BPStream>();
            this.clientGameStream = new GameStream(this.clients);
            this.clientsStream = new BPStream(this.clientGameStream);
            this.gameId = id;
            //this.World = new BPWorld(this);
        }

        private void Run()
        {
            while (true)
            {
                //this.World.Simulate();
                //Thread.Sleep(16);
            }
        }

        public void Start()
        {
            new Thread(Run).Start();
        }

        public void AddPlayer(BPStream player, string username)
        {
            clients.Add(player);
            //init player in world
            //World.AddClient(player);
        }

        public void RemovePlayer(BPStream player)
        {
            clients.Remove(player);
        }

        /// <summary>
        /// Returns a BPStream that writes to all clients.
        /// </summary>
        public BPStream GetBroadcastBPStream()
        {
            return clientsStream;
        }

        public GameStream GetBroadcastStream()
        {
            return clientGameStream;
        }
    }
}