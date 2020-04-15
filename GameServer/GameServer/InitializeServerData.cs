using System;
using System.IO;

namespace GameServer
{
    class InitializeServerData
    {
        static string ServerPathString;
        public static int expRate = 1;
        public static int bonusExp = 1;
        public static int maxPlayerLevel = 100;
        public static int maxPlayerOnline = 30;
        public static int maxGroupSize = 5;
        public static int serverPort;
        public static string serverName = "Server Name";
        public static string serverMotD = "Server Message Of The Day";
        public static string loginMemo = "Login Text";
        public static string serverIP;
        public static string serverPortString = "28015";

        public static void CreateServerWorldDataFiles()
        {
            ServerPathString = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Server.CONF");
            if (!File.Exists(ServerPathString))
            {
                WriteToConsole.SlowlyWriteErrorVarData("Could't find server config file. Created a new one.");
                FileStream fs = File.Open(ServerPathString, FileMode.Append, FileAccess.Write);
                StreamWriter fw = new StreamWriter(fs);
                fw.WriteLine("*** ServerDataFile ***");
                fw.WriteLine();
                fw.WriteLine("*** Exp Rate Which Players Level By. ***");
                fw.WriteLine("*** Default Exp Rate = 1. ***");
                fw.WriteLine("#" + expRate);
                fw.WriteLine();
                fw.WriteLine("*** Bonus Exp Rate Which Players Can Get. ***");
                fw.WriteLine("*** Default Bonus Exp Rate = 1. ***");
                fw.WriteLine("#" + bonusExp);
                fw.WriteLine();
                fw.WriteLine("*** Max Level Players Can Reach. ***");
                fw.WriteLine("*** Default Max Player Level = 100. ***");
                fw.WriteLine("#" + maxPlayerLevel);
                fw.WriteLine();
                fw.WriteLine("*** Max Players That Can Be Online At Once. ***");
                fw.WriteLine("*** Default Max Players Online = 30. ***");
                fw.WriteLine("#" + maxPlayerOnline);
                fw.WriteLine();
                fw.WriteLine("*** Max Amount Of Players Per Group. ***");
                fw.WriteLine("*** Default Max Players Per Group = 5. ***");
                fw.WriteLine("#" + maxGroupSize);
                fw.WriteLine();
                fw.WriteLine("*** Server Name. ***");
                fw.WriteLine("*** Default Server Name = Server Name. ***");
                fw.WriteLine("#" + serverName);
                fw.WriteLine();
                fw.WriteLine("*** Server MotD. ***");
                fw.WriteLine("*** Default Server Message Of The Day = Server Message Of The Day. ***");
                fw.WriteLine("#" + serverMotD);
                fw.WriteLine();
                fw.WriteLine("*** Login Text. ***");
                fw.WriteLine("*** Default Login Text = Login Text. ***");
                fw.WriteLine("#" + loginMemo);
                fw.WriteLine();
                fw.WriteLine("*** Server Ip. ***");
                fw.WriteLine("*** Bind all users to Ip Adress. Default = '0.0.0.0'. ***");
                fw.WriteLine("#0.0.0.0");
                fw.WriteLine();
                fw.WriteLine("*** Server Port. ***");
                fw.WriteLine("*** Default Server Port = '28015'. ***");
                fw.WriteLine("#28015");
                fw.WriteLine();
                fw.Flush();
                fw.Close();
            }
            else
            {
                WriteToConsole.SlowlyWriteServer("Found server config file.", ConsoleColor.Blue);
                return;
            }
        }

        public static void ReadWorldServerDataFiles()
        {
            string[] ServerDataTextHolder = File.ReadAllLines(ServerPathString);
            string[] ServerDataText = new string[19];
            int CurrentLineToAdd = 0;
            for (int i = 0; i < ServerDataTextHolder.Length; i++)
            {
                if (ServerDataTextHolder[i].Contains("#"))
                {
                    ServerDataText[CurrentLineToAdd] = ServerDataTextHolder[i].Replace("#", null);
                    CurrentLineToAdd++;
                }
            }
            Int32.TryParse(ServerDataText[0], out expRate);
            Int32.TryParse(ServerDataText[1], out bonusExp);
            Int32.TryParse(ServerDataText[2], out maxPlayerLevel);
            Int32.TryParse(ServerDataText[3], out maxPlayerOnline);
            Int32.TryParse(ServerDataText[4], out maxGroupSize);
            serverName = ServerDataText[5];
            serverMotD = ServerDataText[6];
            loginMemo = ServerDataText[7];
            serverIP = ServerDataText[8];
            Int32.TryParse(ServerDataText[9], out serverPort);
            ShowServerWorldStats();
        }

        public static void ShowServerWorldStats()
        {
            Console.WriteLine("------------------");
            WriteToConsole.SlowlyWriteServer("Server Stats:", ConsoleColor.Green);
            Console.WriteLine();
            WriteToConsole.writeVarData("Current EXP Rate: *" + expRate + "*.", ConsoleColor.Cyan);
            WriteToConsole.writeVarData("Current Bonus EXP: *" + bonusExp + "*.", ConsoleColor.Cyan);
            WriteToConsole.writeVarData("Maximum Players Online: *" + maxPlayerOnline + "*.", ConsoleColor.Cyan);
            WriteToConsole.writeVarData("Current Max Player Level: *" + maxPlayerLevel + "*.", ConsoleColor.Cyan);
            WriteToConsole.writeVarData("Current Maximun Group Size: *" + maxGroupSize + "*.", ConsoleColor.Cyan);
            WriteToConsole.writeVarData("Server Name: *" + serverName + "*.", ConsoleColor.Cyan);
            WriteToConsole.writeVarData("Server Message Of The Day: *" + serverMotD + "*.", ConsoleColor.Cyan);
            WriteToConsole.writeVarData("Login Memo: *" + loginMemo + "*.", ConsoleColor.Cyan);
            WriteToConsole.writeVarData("Server IP adress: *" + serverIP + "*.", ConsoleColor.Cyan);
            WriteToConsole.writeVarData("Login server port: *" + serverPort + "*.", ConsoleColor.Cyan);
            Console.WriteLine();
            WriteToConsole.writeVarData("This server will use ip: *" + serverIP + "*:*" + serverPort + "*.", ConsoleColor.Cyan);
            Console.WriteLine("------------------");
        }
    }
}
