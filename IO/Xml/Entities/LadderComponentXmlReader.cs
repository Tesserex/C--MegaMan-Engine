using System.Linq;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Entities
{
    internal class LadderComponentXmlReader : IComponentXmlReader
    {
        public string NodeName
        {
            get { return "Ladder"; }
        }

        public IComponentInfo Load(XElement node, Project project)
        {
            var comp = new LadderComponentInfo();
            comp.HitBoxes = node.Elements("Hitbox").Select(GetHitbox).ToList();

            return comp;
        }

        private static HitBoxInfo GetHitbox(XElement boxnode)
        {
            var width = boxnode.GetAttribute<int>("width");
            var height = boxnode.GetAttribute<int>("height");
            var x = boxnode.GetAttribute<int>("x");
            var y = boxnode.GetAttribute<int>("y");

            var box = new HitBoxInfo() {
                Name = boxnode.TryAttribute<string>("name"),
                Box = new Common.Geometry.Rectangle(x, y, width, height),
                ContactDamage = boxnode.TryAttribute<float>("damage"),
                Environment = boxnode.TryAttribute<bool>("environment", true),
                PushAway = boxnode.TryAttribute<bool>("pushaway", true),
                PropertiesName = boxnode.TryAttribute<string>("properties", "Default")
            };
            return box;
        }
    }
}
