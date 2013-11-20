using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MegaMan.Editor
{
    public static class SpriteBitmapCache
    {
        private static Dictionary<string, BitmapImage> images = new Dictionary<string, BitmapImage>();

        private static Dictionary<string, Dictionary<Tuple<int, int, int, int>, WriteableBitmap>> croppedImages = new Dictionary<string, Dictionary<Tuple<int, int, int, int>, WriteableBitmap>>();

        private static Dictionary<string, Dictionary<Tuple<int, int, int, int>, WriteableBitmap>> croppedImagesGrayscale = new Dictionary<string, Dictionary<Tuple<int, int, int, int>, WriteableBitmap>>();

        private static BitmapImage GetOrLoadImage(string absolutePath)
        {
            if (!images.ContainsKey(absolutePath))
            {
                var image = new BitmapImage(new Uri(absolutePath));
                images[absolutePath] = image;
            }

            return images[absolutePath];
        }

        public static WriteableBitmap GetOrLoadFrame(string imagePath, Rectangle srcRect)
        {
            var tuple = Tuple.Create(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);

            if (!croppedImages.ContainsKey(imagePath))
            {
                croppedImages[imagePath] = new Dictionary<Tuple<int, int, int, int>, WriteableBitmap>();
            }

            if (!croppedImages[imagePath].ContainsKey(tuple))
            {
                var source = GetOrLoadImage(imagePath);
                var crop = new CroppedBitmap(source, new Int32Rect(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height));
                crop.Freeze();

                var bmp = BitmapFactory.ConvertToPbgra32Format(crop);

                croppedImages[imagePath][tuple] = bmp;
            }

            return croppedImages[imagePath][tuple];
        }

        public static WriteableBitmap GetOrLoadFrameGrayscale(string imagePath, Rectangle srcRect)
        {
            var tuple = Tuple.Create(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);

            if (!croppedImagesGrayscale.ContainsKey(imagePath))
            {
                croppedImagesGrayscale[imagePath] = new Dictionary<Tuple<int, int, int, int>, WriteableBitmap>();
            }

            if (!croppedImagesGrayscale[imagePath].ContainsKey(tuple))
            {
                var source = GetOrLoadFrame(imagePath, srcRect);
                var grayscale = new FormatConvertedBitmap((BitmapSource)source, PixelFormats.Gray16, BitmapPalettes.Gray256, 1);

                var bmp = BitmapFactory.ConvertToPbgra32Format(grayscale);
                croppedImagesGrayscale[imagePath][tuple] = bmp;
            }

            return croppedImagesGrayscale[imagePath][tuple];
        }
    }
}
