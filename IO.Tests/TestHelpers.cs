using System.IO;
using System.Reflection;

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
