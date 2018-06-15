using System.Linq;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Entities
{
    internal class WeaponComponentXmlReader : IComponentXmlReader
    {
        private readonly MeterXmlReader meterReader;

        public WeaponComponentXmlReader(MeterXmlReader meterReader)
        {
            this.meterReader = meterReader;
        }

        public string NodeName
        {
            get { return "Weapons"; }
        }

        public IComponentInfo Load(XElement node, Project project, IDataSource dataSource)
        {
            var comp = new WeaponComponentInfo();
            comp.Weapons = node.Elements("Weapon")
                .Select(x => {
                    var w = new WeaponInfo {
                        Name = x.GetAttribute<string>("name"),
                        EntityName = x.GetAttribute<string>("entity"),
                        Ammo = x.TryAttribute<int?>("ammo"),
                        Usage = x.TryAttribute<int?>("usage"),
                        Palette = x.TryAttribute<int?>("palette")
                    };

                    var meterNode = x.Element("Meter");
                    if (meterNode != null)
                        w.Meter = meterReader.LoadMeter(meterNode, project.BaseDir, dataSource);

                    return w;
                })
                .ToList();

            return comp;
        }
    }
}
