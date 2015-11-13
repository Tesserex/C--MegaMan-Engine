using MegaMan.Common;
using MegaMan.IO;

namespace MegaMan.Editor.Bll.Factories
{
    public class TilesetDocumentFactory : ITilesetDocumentFactory
    {
        private FactoryCore _core;

        public TilesetDocumentFactory(FactoryCore core)
        {
            _core = core;
        }

        public TilesetDocument Load(FilePath filePath)
        {
            var tilesetReader = _core.Reader.GetTilesetReader(filePath);
            var tileset = tilesetReader.Load(filePath);
            var tilesetDocument = new TilesetDocument(tileset);
            return tilesetDocument;
        }

        public TilesetDocument CreateNew(FilePath filePath)
        {
            var tileset = new Tileset()
            {
                FilePath = filePath,
                TileSize = 16
            };

            var document = new TilesetDocument(tileset);
            AddDefaultTileProperties(document);

            return document;
        }

        private void AddDefaultTileProperties(TilesetDocument tileset)
        {
            tileset.AddBlockProperty();
            tileset.AddSpikeProperty();
            tileset.AddLadderProperty();
            tileset.AddWaterProperty();
        }
    }
}
