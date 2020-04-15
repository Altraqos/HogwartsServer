using System;
using System.Net.Sockets;

namespace GameServer
{
    class Client
    {
        public int connectionID;
        public string playerName = "";
        public int charVal;
        public TcpClient socket;
        public NetworkStream stream;
        private byte[] recBuffer;
        public ByteBuffer buffer;
        public int playerCount;

        public bool isHost = false;

        public void Start()
        {
            //Create a new send and receive buffer size with 4096 bytes.
            socket.SendBufferSize = 4096;
            socket.ReceiveBufferSize = 4096;
            //Set the networkstream to the sockets stream, to allow to write to and from the socket
            stream = socket.GetStream();
            //Create new byte array with the same size as the read and write buffer size, to save the data, and put it in the byte array
            recBuffer = new byte[4096];
            stream.BeginRead(recBuffer, 0, socket.ReceiveBufferSize, OnReceiveData, null);
            //And when the client is connected write the incoming connection from the clients IPAdress
            WriteToConsole.writeVarData("Incoming connection from *" + socket.Client.RemoteEndPoint.ToString(), ConsoleColor.Magenta);
        }

        private void OnReceiveData(IAsyncResult result)
        {
            //Try to read the data the server receives from the client
            try
            {
                int length = stream.EndRead(result);
                //Check if the dataStreams length more is then 0, otherwise disconnect the client from the server, to prevent the server from crashing
                if (length <= 0)
                {
                    CloseConnection();
                    return;
                }
                //Create a new byte array with the length of the incoming datastream, copy the values from the recBuffer to the newBytes array.
                byte[] newBytes = new byte[length];
                Array.Copy(recBuffer, newBytes, length);
                //And send the incoming data to the ServerHandleData class to properly read the bytes out
                ServerHandleData.HandleData(connectionID, newBytes);
                stream.BeginRead(recBuffer, 0, socket.ReceiveBufferSize, OnReceiveData, null);
            }
            //If the server failes to read out the incoming data close the connection to the server.
            catch (Exception ex)
            {
                string[] playerNameHolder = playerName.Split('#');
                WriteToConsole.SlowlyWriteErrorVarData("ERROR: {*" + ex + "*} Connection from *" + playerNameHolder[0] + "* - *" + socket.Client.RemoteEndPoint + "* has been terminated.");
                CloseConnection();
                return;
            }
        }

        private void CloseConnection()
        {
            string[] playerNameHolder = playerName.Split('#');
            DataSend.SendChat(connectionID, playerNameHolder[0] + " has disconnected.");
            DataSend.SendDestroyPlayer(connectionID);
            //Remove the client from the dictonary to prevent crashes to occur when you try to reconnect
            ClientManager.client.Remove(connectionID);
            ClientManager.clientValue.Remove(connectionID);
            //Close the socket and disconnect the client from the server

            WriteToConsole.SlowlyWriteErrorVarData("Connection from *" + playerNameHolder[0] + "* - *" + socket.Client.RemoteEndPoint + "* has been terminated.");
            socket.Close();
        }
    }
}
