using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaMan.Engine
{
    public class Log
    {
        private static readonly string FileNameToPutDebuggingMsg = "debug.txt";

        public static bool SaveLog(string log)
        {
            try
            {
                System.IO.File.AppendAllText(@System.IO.Directory.GetCurrentDirectory() + "\\" + FileNameToPutDebuggingMsg, log + Environment.NewLine);
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}