using System;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    static class ServerTCP
    {
        static TcpListener serverSocket = new TcpListener(IPAddress.Parse(InitializeServerData.serverIP), InitializeServerData.serverPort);

        public static void InitializeNetwork()
        {
            //Start the server up, and start to initialize the server packets, then allow the server to begin accepting clients
            WriteToConsole.SlowlyWriteServer("Initializing Packets...", ConsoleColor.Blue);
            ServerHandleData.InitializePackets();
            serverSocket.Start();
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
        }

        private static void OnClientConnect(IAsyncResult result)
        {
            //If the client has connected to the server, let the server start listening for new clients, and create a new clientClass for the incoming connection
            TcpClient client = serverSocket.EndAcceptTcpClient(result);
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
            ClientManager.CreateNewConnection(client);
        }
    }
}
