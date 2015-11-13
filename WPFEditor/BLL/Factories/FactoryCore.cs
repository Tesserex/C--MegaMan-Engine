using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.IO;

namespace MegaMan.Editor.Bll.Factories
{
    public class FactoryCore
    {
        private readonly IGameLoader _gameLoader;
        public IReaderProvider Reader { get; private set; }

        public FactoryCore(IGameLoader gameLoader)
        {
            _gameLoader = gameLoader;
        }

        public void Load(string filepath)
        {
            Reader = _gameLoader.Load(filepath);
        }
    }
}
