using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Entities.Effects;
using MegaMan.IO.DataSources;
using MegaMan.IO.Xml.Effects;

namespace MegaMan.IO.Xml.Entities
{
    internal class MovementComponentXmlReader : IComponentXmlReader
    {
        private readonly MovementEffectPartXmlReader movementReader;

        public MovementComponentXmlReader(MovementEffectPartXmlReader movementReader)
        {
            this.movementReader = movementReader;
        }

        public string NodeName
        {
            get { return "Movement"; }
        }

        public IComponentInfo Load(XElement node, Project project, IDataSource dataSource)
        {
            return new MovementComponentInfo {
                EffectInfo = (MovementEffectPartInfo)movementReader.Load(node)
            };
        }
    }
}
