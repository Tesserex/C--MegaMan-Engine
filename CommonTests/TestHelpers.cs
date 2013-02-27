using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.Common.Tests
{
    internal static class TestHelpers
    {
        public static string GetInputFile(string filename)
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();

            string path = "MegaMan.Common.Tests.Resources";

            return new StreamReader(thisAssembly.GetManifestResourceStream(path + "." + filename)).ReadToEnd();
        }
    }
}
