using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class SpawnEffectPartXmlReader : IEffectPartXmlReader
    {
        private readonly PositionEffectPartXmlReader positionReader;

        public SpawnEffectPartXmlReader(PositionEffectPartXmlReader positionReader)
        {
            this.positionReader = positionReader;
        }


        public string NodeName
        {
            get
            {
                return "Spawn";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            var info = new SpawnEffectPartInfo();
            info.Name = partNode.GetAttribute<string>("name");
            info.State = partNode.TryAttribute<string>("state", "Start");

            var positionNode = partNode.Element("Position");
            if (positionNode != null)
            {
                info.Position = (PositionEffectPartInfo)positionReader.Load(positionNode);
            }

            return info;
        }
    }
}
