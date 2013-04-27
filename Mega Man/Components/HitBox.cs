using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    public class HitBox
    {
        protected RectangleF box;

        public HitBox(XElement xmlNode)
        {
            float width = xmlNode.GetAttribute<float>("width");

            float height = xmlNode.GetAttribute<float>("height");

            float x = xmlNode.GetAttribute<float>("x");

            float y = xmlNode.GetAttribute<float>("y");

            box = new RectangleF(x, y, width, height);
        }

        public virtual RectangleF BoxAt(PointF offset, bool vflip)
        {
            if (vflip) return new RectangleF(box.X + offset.X, offset.Y - box.Y - box.Height, box.Width, box.Height);
            return new RectangleF(box.X + offset.X, box.Y + offset.Y, box.Width, box.Height);
        }
    }
}
