using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MegaMan.Common;
using MegaMan.IO.DataSources;
using Ninject;

namespace MegaMan.IO
{
    internal static class Extensions
    {
        public static IEnumerable<T> GetImplementersOf<T>()
        {
            return Assembly.GetAssembly(typeof(T))
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(T)) && !t.IsAbstract)
                .Select(t => Injector.Container.Get(t))
                .Cast<T>();
        }

        public static byte[] GetBytesFromFilePath(this IDataSource dataSource, FilePath filePath)
        {
            using (var stream = dataSource.GetData(filePath))
            {
                using (var br = new BinaryReader(stream))
                {
                    return br.ReadBytes((int)stream.Length);
                }
            }
        }
    }
}
