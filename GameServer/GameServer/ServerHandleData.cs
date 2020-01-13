using System.Collections.Generic;

namespace GameServer
{
    class ServerHandleData
    {
        public delegate void Packet(int connectionID, byte[] data);
        public static Dictionary<int, Packet> packets = new Dictionary<int, Packet>();

        public static void InitializePackets()
        {
            packets.Add((int)ClientPackets.CHelloServer, DataReceiver.HandleHelloServer);
            packets.Add((int)ClientPackets.CPlayerPos, DataReceiver.HandlePlayerPos);
            packets.Add((int)ClientPackets.CPlayerName, DataReceiver.HandlePlayerName);
        }

        public static void HandleData(int connectionID, byte[] data)
        {
            byte[] buffer = (byte[])data.Clone();
            int pLength = 0;

            if (ClientManager.client[connectionID].buffer == null)
            {
                ClientManager.client[connectionID].buffer = new ByteBuffer();
            }
            ClientManager.client[connectionID].buffer.WriteBytes(buffer);
            if (ClientManager.client[connectionID].buffer.Count() == 0)
            {
                ClientManager.client[connectionID].buffer.Clear();
                return;
            }
            if (ClientManager.client[connectionID].buffer.Length() >= 4)
            {
                pLength = ClientManager.client[connectionID].buffer.ReadInterger(false);
                if (pLength <= 0)
                {
                    ClientManager.client[connectionID].buffer.Clear();
                    return;
                }
            }

            while (pLength > 0 & pLength <= ClientManager.client[connectionID].buffer.Length() - 4)
            {
                if (pLength <= ClientManager.client[connectionID].buffer.Length() - 4)
                {
                    ClientManager.client[connectionID].buffer.ReadInterger();
                    data = ClientManager.client[connectionID].buffer.ReadBytes(pLength);
                    HandleDataPackets(connectionID, data);
                }
                pLength = 0;
                if (ClientManager.client[connectionID].buffer.Length() >= 4)
                {
                    pLength = ClientManager.client[connectionID].buffer.ReadInterger(false);
                    if (pLength <= 0)
                    {
                        ClientManager.client[connectionID].buffer.Clear();
                        return;
                    }
                }
            }
            if (pLength <= 1)
            {
                ClientManager.client[connectionID].buffer.Clear();
            }
        }

        private static void HandleDataPackets(int connectionID, byte[] data)
        {
            //Create a new ByteBuffer, and start writing the byteArrays data to data the server can use. Convert the bytes to an int to read out its packetID, kill the ByteBuffer and try to read out the packet with the received ID. Then invoke the packet.
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInterger();
            buffer.Dispose();
            if (packets.TryGetValue(packetID, out Packet packet))
            {
                packet.Invoke(connectionID, data);
            }
        }
    }
}
