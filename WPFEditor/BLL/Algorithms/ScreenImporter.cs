using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Common;
using Point = MegaMan.Common.Geometry.Point;

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

        public ScreenInfo Import(BitmapSource image)
        {
            var sourceImage = BitmapFactory.ConvertToPbgra32Format(image);
            var tiles = new List<WriteableBitmap>();
            var coordsToIndex = new Dictionary<Point, int>();

            var tilesize = Stage.Tileset.Tileset.TileSize;

            for (var y = 0; y < image.PixelHeight; y += tilesize)
            {
                for (var x = 0; x < image.PixelWidth; x += tilesize)
                {
                    var tileImage = new WriteableBitmap(tilesize, tilesize, 96, 96, PixelFormats.Pbgra32, null);
                    tileImage.Blit(new Rect(0, 0, tilesize, tilesize), sourceImage, new Rect(x, y, tilesize, tilesize));
                    tiles.Add(tileImage);

                    coordsToIndex[new Point(x / tilesize, y / tilesize)] = tiles.Count - 1;
                }
            }

            var uniqueImages = new List<WriteableBitmap>();
            var imageIndexMap = MapImageIndicesToUnique(tiles, out uniqueImages);
            var imagesToTiles = MatchImagesToTileset(uniqueImages);

            var coordsToTiles = coordsToIndex
                .ToDictionary(p => p.Key, p => imagesToTiles[imageIndexMap[p.Value]]);

            var tileMap = new int[image.PixelWidth / tilesize, image.PixelHeight / tilesize];
            foreach (var point in coordsToTiles)
                tileMap[point.Key.X, point.Key.Y] = point.Value;

            var screen = new ScreenInfo(Stage.FindNextScreenId().ToString(), Stage.Tileset.Tileset);
            var tileLayer = new TileLayer(tileMap, Stage.Tileset.Tileset, 0, 0);
            screen.Layers.Add(new ScreenLayerInfo(screen.Name, tileLayer, false, new List<ScreenLayerKeyframe>()));

            return screen;
        }

        private Dictionary<int, WriteableBitmap> MapImageIndicesToUnique(List<WriteableBitmap> images, out List<WriteableBitmap> keys)
        {
            var comparer = new BitmapComparer();
            var groups = images
                .Select((img, ix) => new { Index = ix, Image = img })
                .GroupBy(a => a.Image, comparer);

            keys = groups.Select(g => g.Key).ToList();

            return groups
                .SelectMany(g => g.Select(a => new {
                    a.Index, First = g.Key }))
                .ToDictionary(a => a.Index, a => a.First);
        }

        private Dictionary<WriteableBitmap, int> MatchImagesToTileset(List<WriteableBitmap> images)
        {
            return images.ToDictionary(i => i, i => MatchImageToTileset(i));
        }

        private int MatchImageToTileset(WriteableBitmap image)
        {
            var scores = Stage.Tileset.Tiles.Select((t) => new { Id = t.Id, Score = GetTileDifferenceScore(image, t) });
            return scores
                .OrderBy(s => s.Score)
                .First()
                .Id;
        }

        private double GetTileDifferenceScore(WriteableBitmap image, Tile tile)
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

            return Math.Abs(scores.Average());
        }
    }
}
