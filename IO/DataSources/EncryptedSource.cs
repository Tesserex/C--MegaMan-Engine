using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common;

namespace MegaMan.IO.DataSources
{
    public class EncryptedSource : IDataSource
    {
        private string _filePath;
        private readonly BundleSource _bundle;

        public string Extension => ".mme";

        public EncryptedSource()
        {
            _bundle = new BundleSource();
        }

        public Stream GetData(FilePath path)
        {
            return _bundle.GetData(path);
        }

        public IEnumerable<FilePath> GetFilesInFolder(FilePath folderPath)
        {
            return _bundle.GetFilesInFolder(folderPath);
        }

        public FilePath GetGameFile()
        {
            return FilePath.FromRelative("game.xml", _filePath);
        }

        public void Init(string path)
        {
            _filePath = path;
            using (var file = File.OpenRead(_filePath))
            using (var br = new BinaryReader(file))
            {
                byte[] cryptBytes = br.ReadBytes((int)file.Length);
                var decrypted = Encryptor.Decrypt(cryptBytes);
                _bundle.Init(path, decrypted);
            }
        }

        public Stream SaveToStream(string projectDirectory)
        {
            using (var zip = _bundle.SaveToStream(projectDirectory))
            using (var br = new BinaryReader(zip))
            {
                byte[] plainBytes = br.ReadBytes((int) zip.Length);
                var encrypted = Encryptor.Encrypt(plainBytes);
                var stream = new MemoryStream();
                var writer = new BinaryWriter(stream);
                writer.Write(encrypted);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }
        }
    }
}
