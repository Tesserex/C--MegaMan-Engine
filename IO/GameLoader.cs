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
    public class GameLoader : IGameLoader
    {
        public IReaderProvider Load(string filepath)
        {
            var ext = Path.GetExtension(filepath);

            if (!Loaders.ContainsKey(ext))
                throw new ArgumentException("The game file is not of a supported type.");

            var loader = Loaders[ext];
            loader.Init(filepath);
            var readerProvider = new ReaderProvider(loader);

            return readerProvider;
        }

        private static Dictionary<string, IDataSourceLoader> Loaders;

        static GameLoader()
        {
            Loaders = Extensions.GetImplementersOf<IDataSourceLoader>()
                .ToDictionary(r => r.Extension);   
        }
    }
}
