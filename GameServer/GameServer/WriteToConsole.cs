using System;

namespace GameServer
{
    class WriteToConsole
    {
        public static void SlowlyWriteErrorVarData(string stringToWrite)
        {
            Console.ForegroundColor = ConsoleColor.White;
            string[] stringHolder = stringToWrite.Split('*');
            for (int i = 0; i < stringHolder.Length; i++)
            {
                if (i % 2 != 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(stringHolder[i]);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(stringHolder[i]);
                }
            }
            if (Char.IsLetter(stringToWrite[stringToWrite.Length - 1]))
                Console.Write(".");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void writeVarData(string stringToWrite, ConsoleColor color)
        {
            Console.ForegroundColor = ConsoleColor.White;
            string[] stringHolder = stringToWrite.Split('*');
            for (int i = 0; i < stringHolder.Length; i++)
            {
                if (i % 2 != 0)
                {
                    Console.ForegroundColor = color;
                    Console.Write(stringHolder[i]);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(stringHolder[i]);
                }
            }
            if (Char.IsLetter(stringToWrite[stringToWrite.Length - 1]))
                Console.Write(".");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void SlowlyWriteServer(string slowlyType, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(slowlyType);
            if (Char.IsLetter(slowlyType[slowlyType.Length - 1]))
                Console.Write(".");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }
    }
}
