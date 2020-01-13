using System;
using System.Threading;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //WriteToLog.ClearFile();
            WriteToConsole.SlowlyWriteServer("Starting the server up", ConsoleColor.Green);
            //WriteToLog.WriteDataToLog("Starting the server up");
            ServerTCP.InitializeNetwork();
            WriteToConsole.SlowlyWriteServer("Server has been started.", ConsoleColor.Green);
            //WriteToLog.WriteDataToLog("Server has been started.");
            Console.ReadKey();
        }
    }
}
