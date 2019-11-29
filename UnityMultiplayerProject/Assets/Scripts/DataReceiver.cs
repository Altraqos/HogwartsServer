using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public enum ServerPackets
    {
        SWelcomeMessage = 1,
        SInstantiatePlayer,
    }

    static class DataReceiver
    {
        public static void HandleWelcomeMessage(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInterger();
            string msg = buffer.ReadString();
            buffer.Dispose();

            Debug.Log(msg);
            DataSender.SendHelloServer();
        }

        public static void HandleInstantiate(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInterger();
            int index = buffer.ReadInterger();
            buffer.Dispose();

            NetworkManager.instance.InstantiatePlayer(index);
        }
    }
}
