using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Editor.Bll
{
    public class EmptySpriteModel : IEntityImage
    {
        private readonly WriteableBitmap _image;

        public EmptySpriteModel(string image)
        {
            _image = BitmapFactory.ConvertToPbgra32Format(SpriteBitmapCache.GetResource(image));
        }

        public int Height => _image.PixelHeight;

        public Point HotSpot => new Point(0, 0);

        public string Name => "No Sprite";

        public int Width => _image.PixelWidth;

        public bool Reversed => false;

        public WriteableBitmap GetImageSource(double zoom)
        {
            return _image.Resize((int)(_image.PixelWidth * zoom), (int)(_image.PixelHeight * zoom), WriteableBitmapExtensions.Interpolation.NearestNeighbor);
        }
    }
}
