using System;

namespace GameServer
{
    public enum ClientPackets
    {
        CHelloServer = 1,
        CPlayerPos,
        CPlayerName,
        CEnemyState,
        CAIPos,
        CChat,
    }

    static class DataReceiver
    {
        public static void HandlePlayerName(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInterger();
            string msg = buffer.ReadString();
            string[] playerName = msg.Split('#');
            ClientManager.client[connectionID].playerName = msg;
            ClientManager.client[connectionID].charVal = Int32.Parse(playerName[1]);
            ClientManager.ShowOnlinePlayers();
            string[] playerNameArray = ClientManager.client[connectionID].playerName.Split('#');
            DataSend.SendChat(connectionID, playerNameArray[0] + " has joined the game.");
            DataSend.SendPlayerName(connectionID, msg, ClientManager.client[connectionID].isHost);
            ClientManager.InstantiatePlayer(connectionID, msg);
        }

        public static void HandleHelloServer(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInterger();
            string msg = buffer.ReadString();
            MFClient(msg);
        }
        public static void HandleChat(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInterger();
            string msg = buffer.ReadString();
            DataSend.SendChat(connectionID, msg);
            string[] msgArray = msg.Split('#');
            WriteToConsole.writeVarData("*" + msgArray[2] + "*" + msgArray[4], ConsoleColor.Yellow);
        }

        public static void HandlePlayerPos(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInterger();
            string msg = buffer.ReadString();
            ClientManager.PositionPlayer(connectionID, msg);
        }
        public static void HandeAIPos(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInterger();
            string msg = buffer.ReadString();
            ClientManager.PositionAI(msg);
        }
        
        public static void HandleEnemyState(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInterger();
            string msg = buffer.ReadString();
            ClientManager.EnemyState(connectionID, msg);
            MFClient(msg);
        }

        static void MFClient(string messageToWrite)
        {
            WriteToConsole.writeVarData("Messsage from client: *" + messageToWrite + "*", ConsoleColor.Yellow);
        }
    }
}
