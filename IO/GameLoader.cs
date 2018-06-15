using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MegaMan.IO.DataSources;

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

        private static readonly Dictionary<string, IDataSource> Loaders;

        static GameLoader()
        {
            Loaders = Extensions.GetImplementersOf<IDataSource>()
                .ToDictionary(r => r.Extension);   
        }
    }
}
