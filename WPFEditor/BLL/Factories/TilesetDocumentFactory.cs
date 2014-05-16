using MegaMan.Common;
using MegaMan.IO;

namespace MegaMan.Editor.Bll.Factories
{
    public class TilesetDocumentFactory : ITilesetDocumentFactory
    {
        private ITilesetReader _reader;

        public TilesetDocumentFactory(ITilesetReader reader)
        {
            _reader = reader;
        }

        public TilesetDocument Load(FilePath filePath)
        {
            var tileset = _reader.Load(filePath);
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
