using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    static class General
    {
        public static void InitializeServer()
        {
            ServerTCP.InitializeNetwork();
            WriteToConsole.SlowlyWriteServer("Server has been started.");
        }
    }
}
