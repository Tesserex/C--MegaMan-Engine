using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    public class HitBox
    {
        protected Rectangle box;

        public HitBox(int x, int y, int width, int height)
        {
            box = new Rectangle(x, y, width, height);
        }

        public virtual Rectangle BoxAt(Point offset, bool vflip)
        {
            if (vflip) return new Rectangle(box.X + offset.X, offset.Y - box.Y - box.Height, box.Width, box.Height);
            return new Rectangle(box.X + offset.X, box.Y + offset.Y, box.Width, box.Height);
        }
    }
}
