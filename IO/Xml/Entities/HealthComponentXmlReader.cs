using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Entities
{
    internal class HealthComponentXmlReader : IComponentXmlReader
    {
        private readonly MeterXmlReader meterReader;

        public HealthComponentXmlReader(MeterXmlReader meterReader)
        {
            this.meterReader = meterReader;
        }

        public string NodeName
        {
            get { return "Health"; }
        }

        public IComponentInfo Load(XElement node, Project project, IDataSource dataSource)
        {
            var comp = new HealthComponentInfo();
            comp.Max = node.TryAttribute("max", node.TryElementValue<float>("Max"));

            comp.StartValue = node.TryAttribute<float?>("startValue");

            var meterNode = node.Element("Meter");
            if (meterNode != null)
            {
                comp.Meter = meterReader.LoadMeter(meterNode, project.BaseDir, dataSource);
            }

            comp.FlashFrames = node.TryAttribute("flash", node.TryElementValue<int>("Flash"));

            return comp;
        }
    }
}
