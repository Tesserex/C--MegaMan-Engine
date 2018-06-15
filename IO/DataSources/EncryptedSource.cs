using System.Collections.Generic;
using System.IO;
using MegaMan.Common;

namespace MegaMan.IO.DataSources
{
    public class EncryptedSource : IDataSource
    {
        private string filePath;
        private readonly BundleSource bundle;

        public string Extension => ".mme";

        public EncryptedSource()
        {
            bundle = new BundleSource();
        }

        public Stream GetData(FilePath path)
        {
            return bundle.GetData(path);
        }

        public IEnumerable<FilePath> GetFilesInFolder(FilePath folderPath)
        {
            return bundle.GetFilesInFolder(folderPath);
        }

        public FilePath GetGameFile()
        {
            return FilePath.FromRelative("game.xml", filePath);
        }

        public void Init(string path)
        {
            filePath = path;
            using (var file = File.OpenRead(filePath))
            using (var br = new BinaryReader(file))
            {
                var cryptBytes = br.ReadBytes((int)file.Length);
                var decrypted = Encryptor.Decrypt(cryptBytes);
                bundle.Init(path, decrypted);
            }
        }

        public Stream SaveToStream(string projectDirectory)
        {
            using (var zip = bundle.SaveToStream(projectDirectory))
            using (var br = new BinaryReader(zip))
            {
                var plainBytes = br.ReadBytes((int) zip.Length);
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
