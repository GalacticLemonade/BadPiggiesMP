using System.Net;
using System.Net.Sockets;

namespace BadPiggiesMP {
    public class BadPiggiesServer {
        //we listen on any ip (0.0.0.0) on port 39182
        private static readonly IPEndPoint listenEndpoint = new IPEndPoint(IPAddress.Any, 39182);
        private readonly TcpListener listenerSocket;

        public BadPiggiesServer() {
            //create the listener socket
            this.listenerSocket = new TcpListener(listenEndpoint);
        }

        public async Task Main() {
            //start listening
            listenerSocket.Start();

            Console.WriteLine($"Listening on {listenEndpoint}!");

            //main server loop
            while (true) {
                //accept a client socket
                TcpClient clientSock = await listenerSocket.AcceptTcpClientAsync();
                //get the client endpoint
                IPEndPoint clientEndpoint = clientSock.Client.RemoteEndPoint as IPEndPoint ?? throw new ArgumentNullException("No endpoint!");
                //log some dbg info
                Console.WriteLine($"{clientEndpoint.Port} connected!");

                //create a worker for that socket
                BadPiggiesClientWorker worker = new BadPiggiesClientWorker(clientSock);

                //and start the worker (running on a new thread)
                worker.Start();
            }
        }
    }
}