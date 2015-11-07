using System;
using System.Collections.Generic;
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
        public Project Load(string filepath)
        {
            var ext = filepath.Substring(filepath.Length - 3);

            if (!Loaders.ContainsKey(ext))
                throw new ArgumentException("The game file is not of a supported type.");

            var loader = Loaders[ext];
            var readerProvider = new ReaderProvider();

            var mainFilePath = loader.GetGameFile();

            var reader = readerProvider.GetProjectReader(mainFilePath);
            var project = reader.Load(loader.GetData(mainFilePath), mainFilePath);

            return project;
        }

        private static Dictionary<string, IDataSourceLoader> Loaders;

        static GameLoader()
        {
            Loaders = Extensions.GetImplementersOf<IDataSourceLoader>()
                .ToDictionary(r => r.Extension);   
        }
    }
}
