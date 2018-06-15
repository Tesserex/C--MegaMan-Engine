using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls.Dialogs;
using MegaMan.Editor.Controls.ViewModels.Dialogs;

namespace MegaMan.Editor.Bll.Algorithms
{
    public class TilesetImporter
    {
        public TilesetDocument Tileset { get; private set; }
        public BitmapSource Tilesheet { get; private set; }

        public List<TilesetImporterError> LastErrors { get; private set; }

        public TilesetImporter(TilesetDocument tileset)
        {
            Tileset = tileset;
            if (Tileset != null)
            {
                Tilesheet = SpriteBitmapCache.GetOrLoadImage(Tileset.SheetPath.Absolute);
            }
        }

        public void ExtractTiles()
        {
            var tempTiles = new List<WriteableBitmap>();
            LastErrors = new List<TilesetImporterError>();

            AddImage(tempTiles, Tilesheet, Tileset.SheetPath.Absolute);

            tempTiles = DeduplicateTemps(tempTiles);
            AppendNewTilesToSheet(tempTiles);
        }

        private void AddImage(List<WriteableBitmap> tempTiles, BitmapSource image, string path)
        {
            var boxModel = new TilesetImageImportDialogViewModel(path);
            var box = new TilesetImageImportDialog();
            box.DataContext = boxModel;
            box.ShowDialog();

            if (box.Result == MessageBoxResult.OK)
            {
                ExtractImage(tempTiles, image, boxModel.Spacing, boxModel.Offset);
            }
        }

        public async Task AddImagesAsync(IEnumerable<TilesetImageImportDialogViewModel> images, IProgress<ProgressDialogState> progress)
        {
            LastErrors = new List<TilesetImporterError>();

            await Task.Run(() => {
                var tempTiles = new List<WriteableBitmap>();

                foreach (var image in images)
                {
                    progress.Report(new ProgressDialogState {
                        ProgressPercentage = 0,
                        Title = "Extracting " + image.FileName
                    });

                    ExtractImage(tempTiles, image.Image, image.Spacing, image.Offset, progress);
                }

                tempTiles = DeduplicateTemps(tempTiles);
                AppendNewTilesToSheet(tempTiles);
            });
        }

        private void ExtractImage(List<WriteableBitmap> tempTiles, BitmapSource image, int spacing, int offset, IProgress<ProgressDialogState> progress = null)
        {
            var sourceImage = BitmapFactory.ConvertToPbgra32Format(image);

            var jump = 16 + spacing;
            var totalTiles = ((image.PixelWidth - offset) / jump) * ((image.PixelHeight - offset) / jump);
            var currentTile = 0;

            for (var y = offset; y < image.PixelHeight; y += jump)
            {
                for (var x = offset; x < image.PixelWidth; x += jump)
                {
                    var tileImage = new WriteableBitmap(16, 16, 96, 96, PixelFormats.Pbgra32, null);
                    tileImage.Blit(new Rect(0, 0, 16, 16), sourceImage, new Rect(x, y, 16, 16));
                    tempTiles.Add(tileImage);

                    currentTile++;
                    if (progress != null)
                    {
                        progress.Report(new ProgressDialogState {
                            ProgressPercentage = currentTile * 100 / totalTiles,
                            Description = string.Format("Extracting {0} / {1}", currentTile, totalTiles)
                        });
                    }
                }
            }
        }

        private List<WriteableBitmap> DeduplicateTemps(List<WriteableBitmap> tempTiles)
        {
            var existingFrames = RipAllFrames();
            var comparer = new BitmapComparer();
            return tempTiles
                .Distinct(comparer)
                .Except(existingFrames.Keys, comparer)
                .ToList();
        }

        private Dictionary<WriteableBitmap, List<SpriteFrame>> RipAllFrames()
        {
            if (Tileset == null)
                return null;

            var threadedSheet = BitmapFactory.ConvertToPbgra32Format(SpriteBitmapCache.GetOrLoadImage(Tileset.SheetPath.Absolute));

            return Tileset.Tiles
                .SelectMany(t => t.Sprite)
                .Select(frame => new { Frame = frame, Image = CutFrame(threadedSheet, frame.SheetLocation) })
                .GroupBy(x => x.Image)
                .ToDictionary(x => x.Key, x => x.Select(a => a.Frame).ToList());
        }

        private WriteableBitmap CutFrame(WriteableBitmap sheet, Rectangle frameRect)
        {
            var wb = new WriteableBitmap(frameRect.Width, frameRect.Height, 96, 96, PixelFormats.Pbgra32, null);
            var srcRect = new Rect(frameRect.X, frameRect.Y, frameRect.Width, frameRect.Height);
            var destRect = new Rect(0, 0, frameRect.Width, frameRect.Height);
            wb.Blit(destRect, sheet, srcRect);
            return wb;
        }

        private void AppendNewTilesToSheet(List<WriteableBitmap> tempTiles)
        {
            var total = tempTiles.Count;
            var tileWidth = Tilesheet.PixelWidth / 16;
            var addedTileHeight = (int)Math.Ceiling(total / (double)tileWidth);

            var tilesheet = new WriteableBitmap(Tilesheet.PixelWidth, Tilesheet.PixelHeight + addedTileHeight * 16, 96, 96, PixelFormats.Pbgra32, null);
            var writeableSource = BitmapFactory.ConvertToPbgra32Format(Tilesheet);
            var originalRect = new Rect(0, 0, Tilesheet.PixelWidth, Tilesheet.PixelHeight);
            tilesheet.Blit(originalRect, writeableSource, originalRect);

            var x = 0;
            var y = Tilesheet.PixelHeight;
            var source = new Rect(0, 0, 16, 16);

            foreach (var frame in tempTiles)
            {
                var dest = new Rect(x, y, 16, 16);
                tilesheet.Blit(dest, frame, source);

                var tile = Tileset.AddTile();
                tile.Sprite[0].SetSheetPosition(x, y);

                if (x < 16 * (tileWidth - 1))
                {
                    x += 16;
                }
                else
                {
                    x = 0;
                    y += 16;
                }
            }

            Tilesheet = tilesheet;
            SpriteBitmapCache.InsertSource(Tileset.SheetPath.Absolute, tilesheet);
            Tileset.RefreshSheet();
        }

        public void CompactTilesheet()
        {
            var allFrames = RipAllFrames();
            var total = allFrames.Count;
            var root = Math.Sqrt(total);
            var width = (int)Math.Ceiling(root);
            var height = (int)Math.Ceiling(total / (double)width);

            var tilesheet = new WriteableBitmap(width * 16, height * 16, 96, 96, PixelFormats.Pbgra32, null);

            var x = 0;
            var y = 0;
            var source = new Rect(0, 0, 16, 16);

            foreach (var frame in allFrames)
            {
                var dest = new Rect(x, y, 16, 16);
                tilesheet.Blit(dest, frame.Key, source);

                foreach (var spriteFrame in frame.Value)
                {
                    spriteFrame.SetSheetPosition(x, y);
                }

                if (x < 16 * (width - 1))
                {
                    x += 16;
                }
                else
                {
                    x = 0;
                    y += 16;
                }
            }
            
            Tilesheet = tilesheet;
            SpriteBitmapCache.InsertSource(Tileset.SheetPath.Absolute, tilesheet);
            Tileset.RefreshSheet();
        }
    }

    public class TilesetImporterError
    {
        public string FilePath { get; set; }
        public string Error { get; set; }
    }
}
