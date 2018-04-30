using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.IO.DataSources
{
    public class BundleSource : IDataSource
    {
        private string _gameFile;

        public string Extension => ".mme";

        public Stream GetData(FilePath path)
        {
            throw new NotImplementedException();
        }

        public FilePath GetGameFile()
        {
            var basePath = Path.GetDirectoryName(_gameFile);
            return FilePath.FromAbsolute(_gameFile, basePath);
        }

        public void Init(string path)
        {
            using (var file = File.OpenRead(path))
            {
                using (var zip = new System.IO.Compression.GZipStream(file, System.IO.Compression.CompressionMode.Decompress))
                {
                    
                }
            }
        }
    }
}
