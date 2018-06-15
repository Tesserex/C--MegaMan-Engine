using System.Xml.Linq;
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
                info.X = ParsePositionBehavior(info, posNodeX);
            if (posNodeY != null)
                info.Y = ParsePositionBehavior(info, posNodeY);

            return info;
        }

        private PositionEffectAxisInfo ParsePositionBehavior(PositionEffectPartInfo info, XElement prop)
        {
            var axisInfo = new PositionEffectAxisInfo();

            var baseAttr = prop.Attribute("base");
            if (baseAttr != null)
            {
                if (baseAttr.Value == "Inherit")
                    axisInfo.Base = null;
                else
                    axisInfo.Base = prop.TryAttribute<float>("base");
            }

            axisInfo.BaseVar = prop.TryAttribute<string>("baseVar");

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

            axisInfo.OffsetVar = prop.TryAttribute<string>("offsetVar");

            return axisInfo;
        }
    }
}
