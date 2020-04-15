using System;
using System.IO;

namespace GameServer
{
    static class WriteToLog
    {
        public static void ClearFile()
        {
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logfile.LOG"), string.Empty);
        }

        public static void WriteDataToLog(string LineToWrite)
        {
            string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logfile.LOG");
            FileStream fs = File.Open(LogPath, FileMode.Append, FileAccess.Write);
            StreamWriter fw = new StreamWriter(fs);
            fw.WriteLine(DateTime.Now.ToString("{ HH:mm } - ") + LineToWrite);
            fw.Flush();
            fw.Close();
        }
    }
}