using System;

namespace GameServer
{
    class WriteToConsole
    {
        public static void SlowlyWriteErrorVarData(string SlowlyType)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(SlowlyType);
            if (Char.IsLetter(SlowlyType[SlowlyType.Length - 1]))
                Console.Write(".");
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
