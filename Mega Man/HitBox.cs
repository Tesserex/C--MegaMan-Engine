using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;
using System.Xml;
using MegaMan;

namespace Mega_Man
{
    public class HitBox
    {
        protected RectangleF box;

        public HitBox(XElement xmlNode)
        {
            float width = xmlNode.GetFloat("width");

            float height = xmlNode.GetFloat("height");

            float x = xmlNode.GetFloat("x");
            
            float y = xmlNode.GetFloat("y");

            box = new RectangleF(x, y, width, height);
        }

        public RectangleF BoxAt(PointF offset, bool vflip)
        {
            if (vflip) return new System.Drawing.RectangleF(box.X + offset.X, offset.Y - box.Y - box.Height, box.Width, box.Height);
            return new System.Drawing.RectangleF(box.X + offset.X, box.Y + offset.Y, box.Width, box.Height);
        }
    }
}
