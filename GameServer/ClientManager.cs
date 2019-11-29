using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    static class ClientManager
    {
        public static Dictionary<int, Client> client = new Dictionary<int, Client>();

        public static void CreateNewConnection(TcpClient tempClient)
        {
            Client newClient = new Client();
            newClient.socket = tempClient;
            newClient.connectionID = ((IPEndPoint)tempClient.Client.RemoteEndPoint).Port;
            newClient.Start();
            client.Add(newClient.connectionID, newClient);

            DataSend.SendWelcomeMessage(newClient.connectionID);
            InstantiatePlayer(newClient.connectionID);
        }

        public static void InstantiatePlayer(int connectionID)
        {
            //Send everyone who is already on the server to the new connection
            foreach (var item in client)
            {
                if (item.Key != connectionID)
                {
                    DataSend.SendInstantiatePlayer(item.Key, connectionID);
                }
            }

            //send the new connection to everyone online
            foreach (var item in client)
            {
                    DataSend.SendInstantiatePlayer(connectionID, item.Key);
            }
        }

        public static void SendDataTo(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.WriteBytes(data);
            client[connectionID].stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            buffer.Dispose();
        }

        public static void SendDataToAll(byte[] data)
        {
            
        }
    }
}
