using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Common.Geometry;

namespace MegaMan.Editor.Bll.Algorithms
{
    class ScreenImporter
    {
        public StageDocument Stage { get; private set; }

        public List<TilesetImporterError> LastErrors { get; private set; }

        public ScreenImporter(StageDocument stage)
        {
            Stage = stage;
        }

        public ScreenDocument Import(BitmapSource image)
        {
            var sourceImage = BitmapFactory.ConvertToPbgra32Format(image);
            var tiles = new List<WriteableBitmap>();
            var coordsToIndex = new Dictionary<Point, int>();

            var jump = 16;

            for (var y = 0; y < image.PixelHeight; y += jump)
            {
                for (var x = 0; x < image.PixelWidth; x += jump)
                {
                    var tileImage = new WriteableBitmap(16, 16, 96, 96, PixelFormats.Pbgra32, null);
                    tileImage.Blit(new System.Windows.Rect(0, 0, 16, 16), sourceImage, new System.Windows.Rect(x, y, 16, 16));
                    tiles.Add(tileImage);

                    coordsToIndex[new Point(x / jump, y / jump)] = tiles.Count - 1;
                }
            }

            var uniqueImages = new List<WriteableBitmap>();
            var imageIndexMap = MapImageIndicesToUnique(tiles, out uniqueImages);
            var imagesToTiles = MatchImagesToTileset(uniqueImages);

            var coordsToTiles = coordsToIndex
                .ToDictionary(p => p.Key, p => imagesToTiles[imageIndexMap[p.Value]]);

            var tileMap = new int[image.PixelWidth / jump, image.PixelHeight / jump];
            foreach (var point in coordsToTiles)
                tileMap[point.Key.X, point.Key.Y] = point.Value;

            var screen = new Common.ScreenInfo("", Stage.Tileset.Tileset);
            var tileLayer = new Common.TileLayer(tileMap, Stage.Tileset.Tileset, 0, 0);
            screen.Layers.Add(new Common.ScreenLayerInfo("", tileLayer, false, new List<Common.ScreenLayerKeyframe>()));

            return new ScreenDocument(screen, Stage);
        }

        private Dictionary<int, WriteableBitmap> MapImageIndicesToUnique(List<WriteableBitmap> images, out List<WriteableBitmap> keys)
        {
            var comparer = new BitmapComparer();
            var groups = images
                .Select((img, ix) => new { Index = ix, Image = img })
                .GroupBy(a => a.Image, comparer);

            keys = groups.Select(g => g.Key).ToList();

            return groups
                .SelectMany(g => g.Select(a => new { Index = a.Index, First = g.Key }))
                .ToDictionary(a => a.Index, a => a.First);
        }

        private Dictionary<WriteableBitmap, int> MatchImagesToTileset(List<WriteableBitmap> images)
        {
            return images.ToDictionary(i => i, i => MatchImageToTileset(i));
        }

        private int MatchImageToTileset(WriteableBitmap image)
        {
            var scores = Stage.Tileset.Tiles.Select((t, index) => new { Index = index, Score = GetTileDifferenceScore(image, t) });
            return scores
                .OrderBy(s => s.Score)
                .First()
                .Index;
        }

        private double GetTileDifferenceScore(WriteableBitmap image, Common.Tile tile)
        {
            var frame = SpriteBitmapCache.GetOrLoadFrame(Stage.Tileset.SheetPath.Absolute, tile.Sprite[0].SheetLocation);

            var scores = new List<int>();
            for (var y = 0; y < frame.PixelHeight; y++)
            {
                for (var x = 0; x < frame.PixelWidth; x++)
                {
                    var imgPixel = image.GetPixel(x, y);
                    var framePixel = frame.GetPixel(x, y);
                    scores.Add((imgPixel.R - framePixel.R) + (imgPixel.G - framePixel.G) + (imgPixel.B - framePixel.B));
                }
            }

            return scores.Average();
        }
    }
}
