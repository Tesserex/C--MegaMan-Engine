using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Entities.Effects;
using MegaMan.IO.Xml.Effects;

namespace MegaMan.IO.Xml.Entities
{
    internal class MovementComponentXmlReader : IComponentXmlReader
    {
        private readonly MovementEffectPartXmlReader _movementReader;

        public MovementComponentXmlReader(MovementEffectPartXmlReader movementReader)
        {
            _movementReader = movementReader;
        }

        public string NodeName
        {
            get { return "Movement"; }
        }

        public IComponentInfo Load(XElement node, Project project)
        {
            return new MovementComponentInfo() {
                EffectInfo = (MovementEffectPartInfo)_movementReader.Load(node)
            };
        }
    }
}
