using System;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

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

            info.Floating = partNode.TryElementValue<bool?>("Floating");
            info.FlipSprite = partNode.TryElementValue<bool?>("FlipSprite");

            info.X = LoadVelocity(partNode.Element("X"));
            info.Y = LoadVelocity(partNode.Element("Y"));
            info.Both = LoadVelocity(partNode.Element("Velocity"));

            return info;
        }

        private VelocityEffectInfo LoadVelocity(XElement prop)
        {
            if (prop == null)
                return null;

            var dir = prop.TryAttribute<string>("direction", "Same");
            return new VelocityEffectInfo() {
                Magnitude = prop.TryAttribute<float?>("magnitude"),
                MagnitudeVarName = prop.TryAttribute<string>("magnitudeVar"),
                Direction = (MovementEffectDirection)Enum.Parse(typeof(MovementEffectDirection), dir)
            };
        }
    }
}
