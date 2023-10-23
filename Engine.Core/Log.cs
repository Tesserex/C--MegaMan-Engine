using System;
using System.IO;

namespace MegaMan.Engine
{
    public class Log
    {
        private static readonly string FileNameToPutDebuggingMsg = "debug.txt";

        public static bool SaveLog(string log)
        {
            try
            {
                File.AppendAllText(Directory.GetCurrentDirectory() + "\\" + FileNameToPutDebuggingMsg, log + Environment.NewLine);
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}