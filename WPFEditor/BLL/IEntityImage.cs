using System.Windows.Media.Imaging;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Editor.Bll
{
    public interface IEntityImage
    {
        int Height { get; }
        Point HotSpot { get; }
        string Name { get; }
        int Width { get; }
        bool Reversed { get; }
        WriteableBitmap GetImageSource(double zoom);
    }
}
