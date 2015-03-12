using System;
using System.Windows.Media.Imaging;
using MegaMan.Common.Geometry;

namespace MegaMan.Editor.Tools
{
    public class ResourceCursor : ImageCursor
    {
        private BitmapImage image;

        public ResourceCursor(string resourceName, Point? hotspot = null)
            : base()
        {
            this.image = new BitmapImage(new Uri("pack://application:,,,/Resources/" + resourceName));
        }

        protected override System.Windows.Media.ImageSource CursorImage
        {
            get
            {
                return this.image;
            }
        }

        protected override float Width { get { return image.PixelWidth; } }
        protected override float Height { get { return image.PixelHeight; } }
        protected override float SnapWidth { get { return 4; } }
        protected override float SnapHeight { get { return 4; } }
    }
}
