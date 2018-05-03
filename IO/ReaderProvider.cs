using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MegaMan.Common;
using MegaMan.IO.DataSources;
using MegaMan.IO.Xml;
using MegaMan.IO.Xml.Handlers.Commands;

namespace MegaMan.IO
{
    internal class ReaderProvider : IReaderProvider
    {
        private readonly IDataSource _dataSource;
        private readonly Dictionary<string, IProjectReader> ProjectReaders;

        public ReaderProvider(IDataSource dataSource)
        {
            _dataSource = dataSource;

            ProjectReaders = Extensions.GetImplementersOf<IProjectReader>()
                .ToDictionary(r => r.Extension);
        }

        public IProjectReader GetProjectReader()
        {
            var mainFilePath = _dataSource.GetGameFile();
            var mainExt = Path.GetExtension(mainFilePath.Relative);

            if (ProjectReaders.ContainsKey(mainExt))
            {
                var reader = ProjectReaders[mainExt];
                reader.Init(_dataSource);
                return reader;
            }

            throw new ArgumentException("The game file is not of a supported type.");
        }

        public IRawReader GetRawReader()
        {
            var reader = new RawReader();
            reader.Init(_dataSource);
            return reader;
        }

        public IStageReader GetStageReader(FilePath path)
        {
            var reader = new StageXmlReader(this, new EntityPlacementXmlReader(), new HandlerCommandXmlReader());
            reader.Init(_dataSource);
            return reader;
        }

        public ITilesetReader GetTilesetReader(FilePath path)
        {
            var reader = new TilesetXmlReader();
            reader.Init(_dataSource);
            return reader;
        }
    }
}
