using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Effects
{
    internal class MovementEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Movement";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            var info = new MovementEffectPartInfo();

            var flyNode = partNode.Element("Flying");
            if (flyNode != null)
                info.Flying = flyNode.GetValue<bool>();

            var flipNode = partNode.Element("FlipSprite");
            if (flipNode != null)
                info.FlipSprite = flipNode.GetValue<bool>();

            info.X = LoadVelocity(partNode.Element("X"));
            info.Y = LoadVelocity(partNode.Element("Y"));
            info.Both = LoadVelocity(partNode.Element("Velocity"));

            return info;
        }

        private VelocityEffectInfo LoadVelocity(XElement prop)
        {
            var dir = prop.TryAttribute<string>("direction", "Same");
            return new VelocityEffectInfo() {
                Magnitude = prop.TryAttribute<float?>("magnitude"),
                Direction = (MovementEffectDirection)Enum.Parse(typeof(MovementEffectDirection), dir)
            };
        }
    }
}
