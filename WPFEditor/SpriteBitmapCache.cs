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
    public class SpriteBitmapCache
    {
        private static Dictionary<string, BitmapImage> images = new Dictionary<string, BitmapImage>();

        private static Dictionary<string, Dictionary<Tuple<int, int, int, int>, CroppedBitmap>> croppedImages = new Dictionary<string, Dictionary<Tuple<int, int, int, int>, CroppedBitmap>>();

        private static BitmapImage GetOrLoadImage(string absolutePath)
        {
            if (!images.ContainsKey(absolutePath))
            {
                var image = new BitmapImage(new Uri(absolutePath));
                images[absolutePath] = image;
            }

            return images[absolutePath];
        }

        public static ImageSource GetOrLoadFrame(string imagePath, Rectangle srcRect)
        {
            var tuple = Tuple.Create(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);

            if (!croppedImages.ContainsKey(imagePath))
            {
                croppedImages[imagePath] = new Dictionary<Tuple<int, int, int, int>, CroppedBitmap>();
            }

            if (!croppedImages[imagePath].ContainsKey(tuple))
            {
                var source = GetOrLoadImage(imagePath);
                var crop = new CroppedBitmap(source, new Int32Rect(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height));
                crop.Freeze();

                croppedImages[imagePath][tuple] = crop;
            }

            return croppedImages[imagePath][tuple];
        }
    }
}
