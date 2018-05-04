using System.Collections.Generic;
using System.IO;
using System.Linq;
using MegaMan.Common;

namespace MegaMan.IO.DataSources
{
    internal abstract class FileSource : IDataSource
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

        public IEnumerable<FilePath> GetFilesInFolder(FilePath folderPath)
        {
            var basePath = Path.GetDirectoryName(_gameFile);
            return Directory.EnumerateFiles(folderPath.Absolute, "*" + Extension, SearchOption.AllDirectories)
                .Select(p => FilePath.FromRelative(p, basePath));
        }
    }
}
