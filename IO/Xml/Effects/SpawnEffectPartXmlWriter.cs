using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class SpawnEffectPartXmlWriter : IEffectPartXmlWriter
    {
        private readonly PositionEffectPartXmlWriter _positionWriter;

        public SpawnEffectPartXmlWriter(PositionEffectPartXmlWriter positionWriter)
        {
            _positionWriter = positionWriter;
        }

        public Type EffectPartType
        {
            get
            {
                return typeof(SpawnEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var spawn = (SpawnEffectPartInfo)info;
            writer.WriteStartElement("Spawn");

            writer.WriteAttributeString("name", spawn.Name);
            writer.WriteAttributeString("state", spawn.State);

            _positionWriter.Write(spawn.Position, writer);

            writer.WriteEndElement();
        }
    }
}
