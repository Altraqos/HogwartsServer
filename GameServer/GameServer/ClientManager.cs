using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    static class ClientManager
    {
        public static Dictionary<int, Client> client = new Dictionary<int, Client>();
        public static Dictionary<int, Client> clientCount = new Dictionary<int, Client>();
        public static List<int> clientValue = new List<int>();

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
            clientValue.Add(newClient.connectionID);
            if (clientValue[0] == newClient.connectionID)
                client[newClient.connectionID].isHost = true;
            //Send the welcomeMessage to the client, and send the instantiate to every player online
            DataSend.SendWelcomeMessage(newClient.connectionID);
            //And write the new connection to the server
            Console.WriteLine("------------------");
            WriteToConsole.writeVarData("Creating a new connection, client. ID: *" + newClient.connectionID + "*.", ConsoleColor.Green);
        }

        public static void ShowOnlinePlayers()
        {
            Console.WriteLine("------------------");
            if (client.Count < 2)
                WriteToConsole.writeVarData("Currently there is: *" + client.Count + "* player online", ConsoleColor.Green);
            else
                WriteToConsole.writeVarData("Currently there are: *" + client.Count + "* players online", ConsoleColor.Green);
            Console.WriteLine("------------------");
            foreach (var item in client)
            {
                string[] playerName = client[item.Key].playerName.Split('#');
                WriteToConsole.writeVarData("Player Count: *" + client[item.Key].playerCount + "* | ID: *" + item.Key.ToString() + "* | Name: *" + playerName[0] + "* | Character Value: *" + client[item.Key].charVal + "* | Is Host: *" + client[item.Key].isHost + "*", ConsoleColor.Green);
            }
            Console.WriteLine("------------------");
        }

        public static void InstantiatePlayer(int connectionID, string playerName)
        {
            //Send everyone who is already on the server to the new connection
            foreach (var item in client)
            {
                if (item.Key != connectionID)
                    DataSend.SendInstantiatePlayer(item.Key, connectionID, client[item.Key].playerName);
            }          
            //send the new connection to everyone online
            foreach (var item in client)
                DataSend.SendInstantiatePlayer(connectionID, item.Key, playerName);
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

        public static void PositionAI(string AIPos)
        {
            foreach (var item in client)
            {
                if (!client[item.Key].isHost)
                    DataSend.SendAIPos(item.Key, AIPos);
            }
        }
        
        public static void EnemyState(int connectionID, string enemyState)
        {
            //Split the incoming string
            string[] stringholder = enemyState.Split('#');
            //Send everyone who is already on the server to the new position, but not the client who sent the data
            foreach (var item in client)
            {
                if (item.Key.ToString() != stringholder[0])
                    DataSend.SendEnemyState(item.Key, enemyState);
            }
        }
        public static void NamePlayer(int connectionID, string playerName)
        {
            //Send everyone who is already on the server to the new name
            foreach (var item in client)
                DataSend.SendPlayerName(item.Key, client[item.Key].playerName, client[item.Key].isHost);
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
