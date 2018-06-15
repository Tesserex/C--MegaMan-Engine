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
        private readonly IDataSource dataSource;
        private readonly Dictionary<string, IProjectReader> projectReaders;

        public ReaderProvider(IDataSource dataSource)
        {
            this.dataSource = dataSource;

            projectReaders = Extensions.GetImplementersOf<IProjectReader>()
                .ToDictionary(r => r.Extension);
        }

        public IProjectReader GetProjectReader()
        {
            var mainFilePath = dataSource.GetGameFile();
            var mainExt = Path.GetExtension(mainFilePath.Relative);

            if (projectReaders.ContainsKey(mainExt))
            {
                var reader = projectReaders[mainExt];
                reader.Init(dataSource);
                return reader;
            }

            throw new ArgumentException("The game file is not of a supported type.");
        }

        public IRawReader GetRawReader()
        {
            var reader = new RawReader();
            reader.Init(dataSource);
            return reader;
        }

        public IStageReader GetStageReader(FilePath path)
        {
            var reader = new StageXmlReader(this, new EntityPlacementXmlReader(), new HandlerCommandXmlReader());
            reader.Init(dataSource);
            return reader;
        }

        public ITilesetReader GetTilesetReader(FilePath path)
        {
            var reader = new TilesetXmlReader();
            reader.Init(dataSource);
            return reader;
        }
    }
}
