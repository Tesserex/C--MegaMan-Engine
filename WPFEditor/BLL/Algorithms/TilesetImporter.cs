using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Common;

namespace MegaMan.Editor.Bll.Algorithms
{
    public class TilesetImporter
    {
        public Tileset Tileset { get; private set; }

        public List<TilesetImporterError> LastErrors { get; private set; }

        private List<WriteableBitmap> _tempTiles;
        private List<WriteableBitmap> _existingFrames;

        public TilesetImporter(Tileset tileset)
        {
            Tileset = tileset;
            if (Tileset != null)
                _existingFrames = RipAllFrames();
        }

        public void AddImages(IEnumerable<string> filePaths)
        {
            _tempTiles = new List<WriteableBitmap>();
            LastErrors = new List<TilesetImporterError>();

            var images = filePaths
                .Where(p => System.IO.File.Exists(p))
                .Select(p => new BitmapImage(new Uri(p)));

            foreach (var i in images)
                AddImage(i);

            DeduplicateTemps();
        }

        private void AddImage(BitmapImage image)
        {
            if (image.PixelWidth % 16 != 0 || image.PixelHeight % 16 != 0)
            {
                LastErrors.Add(new TilesetImporterError() {
                    FilePath = image.UriSource.AbsoluteUri,
                    Error = "The selected image must have a size of a multiple of 16x16."
                });

                return;
            }

            var sourceImage = BitmapFactory.ConvertToPbgra32Format(image);

            for (var y = 0; y < (image.PixelHeight / 16); y += 16)
            {
                for (var x = 0; x < (image.PixelWidth / 16); x += 16)
                {
                    var tileImage = new WriteableBitmap(16, 16, 96, 96, PixelFormats.Pbgra32, null);
                    tileImage.Blit(new System.Windows.Rect(0, 0, 16, 16), sourceImage, new System.Windows.Rect(x, y, 16, 16));
                    tileImage.Freeze();
                    _tempTiles.Add(tileImage);
                }
            }
        }

        private void DeduplicateTemps()
        {
            var comparer = new BitmapComparer();
            _tempTiles = _tempTiles
                .Distinct(comparer)
                .Except(_existingFrames, comparer)
                .ToList();
        }

        private List<WriteableBitmap> RipAllFrames()
        {
            if (Tileset == null)
                return null;

            return Tileset
                .SelectMany(t => t.Sprite.Select(f => f.SheetLocation))
                .Select(rect => SpriteBitmapCache.GetOrLoadFrame(Tileset.SheetPath.Absolute, rect))
                .ToList();
        }

        private void ReconstructTilesheet()
        {
            var root = Math.Sqrt(_existingFrames.Count);
            var width = (int)Math.Ceiling(root);
            var height = (int)Math.Ceiling(_existingFrames.Count / (double)width);

            var tilesheet = new WriteableBitmap(width * 16, height * 16, 96, 96, PixelFormats.Pbgra32, null);
        }

        private class BitmapComparer : IEqualityComparer<WriteableBitmap>
        {
            public bool Equals(WriteableBitmap x, WriteableBitmap y)
            {
                var xBytes = x.ToByteArray();
                var yBytes = y.ToByteArray();

                return xBytes.SequenceEqual(yBytes);
            }

            public int GetHashCode(WriteableBitmap obj)
            {
                return obj.GetHashCode();
            }
        }
    }

    public class TilesetImporterError
    {
        public string FilePath { get; set; }
        public string Error { get; set; }
    }
}
