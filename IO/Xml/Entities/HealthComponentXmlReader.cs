using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Entities
{
    internal class HealthComponentXmlReader : IComponentXmlReader
    {
        public string NodeName
        {
            get { return "Health"; }
        }

        public IComponentInfo Load(XElement node, Project project)
        {
            var comp = new HealthComponentInfo();
            comp.Max = node.TryAttribute<float>("max", node.TryElementValue<float>("Max"));

            comp.StartValue = node.TryAttribute<float?>("startValue");

            XElement meterNode = node.Element("Meter");
            if (meterNode != null)
            {
                comp.Meter = HandlerXmlReader.LoadMeter(meterNode, project.BaseDir);
            }

            comp.FlashFrames = node.TryAttribute("flash", node.TryElementValue<int>("Flash"));

            return comp;
        }
    }
}
