using System.IO;
using MegaMan.Common;

namespace MegaMan.IO.DataSources
{
    internal abstract class FileSourceLoader : IDataSourceLoader
    {
        private string _gameFile;

        public abstract string Extension
        {
            get;
        }

        public void Init(string path)
        {
            _gameFile = path;
        }

        public Stream GetData(FilePath path)
        {
            return File.OpenRead(path.Absolute);
        }

        public FilePath GetGameFile()
        {
            var basePath = Path.GetDirectoryName(_gameFile);
            return FilePath.FromAbsolute(_gameFile, basePath);
        }
    }
}
