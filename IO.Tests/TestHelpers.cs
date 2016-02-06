using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.IO.Tests
{
    internal static class TestHelpers
    {
        public static string GetInputFile(string filename)
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();

            return new StreamReader(thisAssembly.GetManifestResourceStream(thisAssembly.GetName().Name + ".Resources." + filename)).ReadToEnd();
        }
    }
}
