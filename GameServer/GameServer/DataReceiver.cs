using System;

namespace GameServer
{
    public enum ClientPackets
    {
        CHelloServer = 1,
        CPlayerPos,
        CPlayerName,
    }


    static class DataReceiver
    {
        public static void HandlePlayerName(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInterger();
            string msg = buffer.ReadString();
            int charVal = buffer.ReadInterger();
            ClientManager.client[connectionID].playerName = msg;
            ClientManager.client[connectionID].charVal = charVal;
            ClientManager.ShowOnlinePlayers();
            DataSend.SendPlayerName(connectionID, msg);
            ClientManager.InstantiatePlayer(connectionID, msg);
            MFClient("PlayerName: " + msg + "- CharVal: " + charVal);
        }

        public static void HandleHelloServer(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInterger();
            string msg = buffer.ReadString();
            MFClient(msg);
        }

        public static void HandlePlayerPos(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInterger();
            string msg = buffer.ReadString();
            ClientManager.PositionPlayer(connectionID, msg);
        }

        static void MFClient(string messageToWrite)
        {
            Console.WriteLine("Messsage from client: '" + messageToWrite + "'");
        }
    }
}
