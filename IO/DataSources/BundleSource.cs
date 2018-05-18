using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.IO.DataSources
{
    public class BundleSource : IDataSource
    {
        private string _zipFile;
        private byte[] _zipContents;

        public string Extension => ".zip";

        public Stream GetData(FilePath path)
        {
            using (var mem = new MemoryStream(_zipContents))
            {
                using (var zip = new ZipArchive(mem, ZipArchiveMode.Read))
                {
                    var memoryStream = new MemoryStream();

                    var zipPathBack = path.Relative.ToUpper().Replace('/', '\\');
                    var zipPathForward = path.Relative.ToUpper().Replace('\\', '/');

                    // do case insensitive comparison, Entry() is sensitive
                    var entry = zip.Entries.SingleOrDefault(e => e.FullName.ToUpper() == zipPathBack || e.FullName.ToUpper() == zipPathForward);

                    var zipStream = entry.Open();
                    zipStream.CopyTo(memoryStream);
                    zipStream.Close();
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return memoryStream;
                }
            }
        }

        public IEnumerable<FilePath> GetFilesInFolder(FilePath folderPath)
        {
            using (var mem = new MemoryStream(_zipContents))
            {
                using (var zip = new ZipArchive(mem, ZipArchiveMode.Read))
                {
                    return zip.Entries.Where(x => x.FullName.StartsWith(folderPath.Relative))
                        .Select(x => FilePath.FromRelative(x.FullName, _zipFile));
                }
            }
        }

        public FilePath GetGameFile()
        {
            return FilePath.FromRelative("game.xml", _zipFile);
        }

        public void Init(string path)
        {
            _zipFile = path;

            using (var file = File.OpenRead(_zipFile))
            {
                using (BinaryReader br = new BinaryReader(file))
                {
                    _zipContents = br.ReadBytes((int)file.Length);
                }
            }
        }

        public void Init(string file, byte[] bytes)
        {
            _zipFile = file;
            _zipContents = bytes;
        }

        public Stream SaveToStream(string projectDirectory)
        {
            var mem = new MemoryStream();
            using (var zip = new ZipArchive(mem, ZipArchiveMode.Create, true))
            {
                foreach (var file in Directory.GetFiles(projectDirectory, "*", SearchOption.AllDirectories))
                {
                    zip.CreateEntryFromFile(file, FilePath.FromAbsolute(file, projectDirectory).Relative);
                }
            }

            mem.Position = 0;
            return mem;
        }
    }
}
