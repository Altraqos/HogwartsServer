using System;

namespace GameServer
{
    public enum ServerPackets
    {
        SWelcomeMessage = 1,
        SPlayerData,
        SPlayerPos,
        SPlayerDestroy,
        SPlayerName,
        SEnemyState,
        SAIPos,
        SChat,
    }

    static class DataSend
    {
        public static void SendWelcomeMessage(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ServerPackets.SWelcomeMessage);
            buffer.WriteString(InitializeServerData.loginMemo);
            buffer.WriteInterger(connectionID);
            ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void SendChat(int connectionID, string message)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ServerPackets.SChat);
            buffer.WriteString(message);
            foreach (var item in ClientManager.client)
            {
                ClientManager.SendDataTo(item.Key, buffer.ToArray());
            }
            buffer.Dispose();
        }

        public static void SendDestroyPlayer(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ServerPackets.SPlayerDestroy);
            buffer.WriteString("Destroyed Player with ID {" + connectionID + "}");
            buffer.WriteInterger(connectionID);
            foreach (var item in ClientManager.client)
            {
                if (item.Key != connectionID)
                    ClientManager.SendDataTo(item.Key, buffer.ToArray());
            }
            buffer.Dispose();
        }

        public static void SendInstantiatePlayer(int index, int connectionID, string playerName)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ServerPackets.SPlayerData);
            buffer.WriteInterger(index);
            buffer.WriteString(playerName);
            if (index != connectionID)
                ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendPlayerPos(int connectionID, string playerPos)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ServerPackets.SPlayerPos);
            buffer.WriteString(playerPos);
            ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void SendAIPos(int connectionID, string AIPos)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ServerPackets.SAIPos);
            buffer.WriteString(AIPos);
            ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void SendEnemyState(int connectionID, string enemyState)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ServerPackets.SEnemyState);
            buffer.WriteString(enemyState);
            ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendPlayerName(int connectionID, string playerName, bool isHost)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ServerPackets.SPlayerName);
            buffer.WriteString(connectionID + "#" + playerName);
            buffer.WriteBool(isHost);
            ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }
    }
}
