using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class PositionEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Position";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            var info = new PositionEffectPartInfo();

            var posNodeX = partNode.Element("X");
            var posNodeY = partNode.Element("Y");

            if (posNodeX != null)
                ParsePositionBehavior(info, posNodeX, Axis.X);
            if (posNodeY != null)
                ParsePositionBehavior(info, posNodeY, Axis.Y);

            return info;
        }

        private void ParsePositionBehavior(PositionEffectPartInfo info, XElement prop, Axis axis)
        {
            var axisInfo = new PositionEffectAxisInfo();

            XAttribute baseAttr = prop.Attribute("base");
            if (baseAttr != null)
            {
                if (baseAttr.Value == "Inherit")
                    axisInfo.Base = null;
                else
                    axisInfo.Base = prop.TryAttribute<float>("base");
            }

            if (prop.Attribute("offset") != null)
            {
                axisInfo.Offset = prop.TryAttribute<float?>("offset");

                try
                {
                    axisInfo.OffsetDirection = prop.GetAttribute<OffsetDirection>("direction");
                } catch
                {
                    throw new GameXmlException(prop, "Position offset direction was not valid!");
                }
            }

            if (axis == Axis.X)
                info.X = axisInfo;
            else if (axis == Axis.Y)
                info.Y = axisInfo;
        }
    }
}
