namespace Assets.Scripts
{
    public enum ClientPackets
    {
        CHelloServer = 1,
        CPlayerPos,
        CPlayerName,
    }


    static class DataSender
    {
        public static void SendPlayerName(string PlayerName, int charValue)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ClientPackets.CPlayerName);
            buffer.WriteString(PlayerName);
            buffer.WriteInterger(charValue);
            ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }


        public static void SendHelloServer(string playerName)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ClientPackets.CHelloServer);
            buffer.WriteString("Thank you I am now connected to you!");
            ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();     
        }

        public static void SendPlayerPos(string playerPosXYZ)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ClientPackets.CPlayerPos);
            buffer.WriteString(playerPosXYZ);
            ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
    }
}
