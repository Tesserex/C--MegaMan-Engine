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
            float width;
            XAttribute widthAttr = xmlNode.Attribute("width");
            if (widthAttr != null)
            {
                bool s = widthAttr.Value.TryParse(out width);
                if (!s) throw new EntityXmlException(widthAttr, "Hitbox width was not a valid number!");
            }
            else throw new EntityXmlException(xmlNode, "Hitbox does not specify width!");

            float height;
            XAttribute heightAttr = xmlNode.Attribute("height");
            if (heightAttr != null)
            {
                bool s = heightAttr.Value.TryParse(out height);
                if (!s) throw new EntityXmlException(heightAttr, "Hitbox height was not a valid number!");
            }
            else throw new EntityXmlException(xmlNode, "Hitbox does not specify height!");

            float x;
            XAttribute xAttr = xmlNode.Attribute("x");
            if (xAttr != null)
            {
                bool s = xAttr.Value.TryParse(out x);
                if (!s) throw new EntityXmlException(xAttr, "Hitbox x was not valid!");
            }
            else throw new EntityXmlException(xmlNode, "Hitbox does not specify x position!");

            float y;
            XAttribute yAttr = xmlNode.Attribute("y");
            if (yAttr != null)
            {
                bool s = yAttr.Value.TryParse(out y);
                if (!s) throw new EntityXmlException(yAttr, "Hitbox y was not valid!");
            }
            else throw new EntityXmlException(xmlNode, "Hitbox does not specify y position!");

            box = new RectangleF(x, y, width, height);
        }

        public RectangleF BoxAt(PointF offset, bool vflip)
        {
            if (vflip) return new System.Drawing.RectangleF(box.X + offset.X, offset.Y - box.Y - box.Height, box.Width, box.Height);
            return new System.Drawing.RectangleF(box.X + offset.X, box.Y + offset.Y, box.Width, box.Height);
        }
    }
}
