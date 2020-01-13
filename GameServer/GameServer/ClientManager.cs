using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    static class ClientManager
    {
        public static Dictionary<int, Client> client = new Dictionary<int, Client>();

        public static void CreateNewConnection(TcpClient tempClient)
        {
            //Create a new client from the tenpClient.
            Client newClient = new Client();
            newClient.socket = tempClient;
            //Set the client's connectionID to the connected Port
            newClient.connectionID = ((IPEndPoint)tempClient.Client.RemoteEndPoint).Port;
            newClient.Start();
            //Add a new client to the dictionary, the key is the connectionID, the value the client
            client.Add(newClient.connectionID, newClient);
            //Send the welcomeMessage to the client, and send the instantiate to every player online
            DataSend.SendWelcomeMessage(newClient.connectionID);
            //And write the new connection to the server
            Console.WriteLine("Creating a new connection, client. ID:  '" + newClient.connectionID + "'.");
            //WriteToLog.WriteDataToLog("Creating a new connection, client. ID:  '" + newClient.connectionID + "'.");
        }

        public static void ShowOnlinePlayers()
        {
            Console.WriteLine("Currently there are: '" + client.Count + "' players online");
            Console.WriteLine("------------------");
            foreach (var item in client)
            {
                Console.WriteLine("ID: '" + item.Key.ToString() + "' | Name: '" + client[item.Key].playerName + "' | charVal: '" + client[item.Key].charVal + "'");
            }
            Console.WriteLine("------------------");
        }

        public static void InstantiatePlayer(int connectionID, string playerName)
        {
            //Send everyone who is already on the server to the new connection
            foreach (var item in client)
            {
                if (item.Key != connectionID)
                {
                    DataSend.SendInstantiatePlayer(item.Key, connectionID, client[item.Key].playerName, client[item.Key].charVal);
                }
            }
            
            //send the new connection to everyone online
            foreach (var item in client)
            {
                DataSend.SendInstantiatePlayer(connectionID, item.Key, playerName, client[item.Key].charVal);
            }
        }

        public static void PositionPlayer(int connectionID, string playerPos)
        {
            //Split the incoming string
            string[] stringholder = playerPos.Split('#');
            //Send everyone who is already on the server to the new position, but not the client who sent the data
            foreach (var item in client)
            {
                if (item.Key.ToString() != stringholder[0])
                    DataSend.SendPlayerPos(item.Key, playerPos);
            }
        }
        public static void NamePlayer(int connectionID, string playerName)
        {
            //Send everyone who is already on the server to the new name
            foreach (var item in client)
            {
                DataSend.SendPlayerName(item.Key, client[item.Key].playerName);
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
    }
}
