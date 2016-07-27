using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaMan.Engine
{
    class Log
    {
        public static bool saveLog(string log)
        {
            try
            {
                System.IO.File.AppendAllText(@System.IO.Directory.GetCurrentDirectory() + "\\" + Constants.Paths.FileNameToPutDebuggingMsg, log + Environment.NewLine);
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}