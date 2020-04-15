using System;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteToConsole.SlowlyWriteServer("Looking for server config file.", ConsoleColor.Green);
            InitializeServerData.CreateServerWorldDataFiles();
            InitializeServerData.ReadWorldServerDataFiles();
            WriteToConsole.SlowlyWriteServer("Starting the server up.", ConsoleColor.Green);
            ServerTCP.InitializeNetwork();
            WriteToConsole.SlowlyWriteServer("Server has been started.", ConsoleColor.Green);
            Console.ReadLine();
        }
    }
}
