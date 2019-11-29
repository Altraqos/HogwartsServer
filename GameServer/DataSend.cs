using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public enum ServerPackets
    {
        SWelcomeMessage = 1,
        SPlayerData,
    }

    static class DataSend
    {
        public static void SendWelcomeMessage(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ServerPackets.SWelcomeMessage);
            buffer.WriteString("Hello! Welcome to the server!");
            ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendInstantiatePlayer(int index, int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInterger((int)ServerPackets.SPlayerData);
            buffer.WriteInterger(index);
            ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }
    }
}
