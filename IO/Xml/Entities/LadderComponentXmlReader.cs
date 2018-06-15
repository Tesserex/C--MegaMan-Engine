using System.Linq;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Entities
{
    internal class LadderComponentXmlReader : IComponentXmlReader
    {
        public string NodeName
        {
            get { return "Ladder"; }
        }

        public IComponentInfo Load(XElement node, Project project, IDataSource dataSource)
        {
            var comp = new LadderComponentInfo();
            comp.HitBoxes = node.Elements("Hitbox").Select(GetHitbox).ToList();

            return comp;
        }

        private static HitBoxInfo GetHitbox(XElement boxnode)
        {
            var width = boxnode.GetAttribute<float>("width");
            var height = boxnode.GetAttribute<float>("height");
            var x = boxnode.GetAttribute<float>("x");
            var y = boxnode.GetAttribute<float>("y");

            var box = new HitBoxInfo() {
                Name = boxnode.TryAttribute<string>("name"),
                Box = new Common.Geometry.RectangleF(x, y, width, height),
                ContactDamage = boxnode.TryAttribute<float>("damage"),
                Environment = boxnode.TryAttribute<bool>("environment", true),
                PushAway = boxnode.TryAttribute<bool>("pushaway", true),
                PropertiesName = boxnode.TryAttribute<string>("properties", "Default")
            };
            return box;
        }
    }
}
