using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Entities
{
    internal class CollisionComponentXmlReader : IComponentXmlReader
    {
        public string NodeName
        {
            get { return "Collision"; }
        }

        public IComponentInfo Load(XElement node, Project project, IDataSource dataSource)
        {
            var component = new CollisionComponentInfo();

            foreach (var boxnode in node.Elements("Hitbox"))
            {
                var box = GetHitbox(boxnode);

                foreach (var groupnode in boxnode.Elements("Hits"))
                    box.Hits.Add(groupnode.Value);

                foreach (var groupnode in boxnode.Elements("Group"))
                    box.Groups.Add(groupnode.Value);

                foreach (var resistNode in boxnode.Elements("Resist"))
                {
                    var resistName = resistNode.GetAttribute<string>("name");
                    var mult = resistNode.GetAttribute<float>("multiply");
                    box.Resistance.Add(resistName, mult);
                }

                component.HitBoxes.Add(box);
            }

            component.Enabled = node.TryAttribute<bool>("Enabled");

            return component;
        }

        private static HitBoxInfo GetHitbox(XElement boxnode)
        {
            var width = boxnode.GetAttribute<float>("width");
            var height = boxnode.GetAttribute<float>("height");
            var x = boxnode.GetAttribute<float>("x");
            var y = boxnode.GetAttribute<float>("y");

            var box = new HitBoxInfo {
                Name = boxnode.TryAttribute<string>("name"),
                Box = new RectangleF(x, y, width, height),
                ContactDamage = boxnode.TryAttribute<float>("damage"),
                Environment = boxnode.TryAttribute("environment", true),
                PushAway = boxnode.TryAttribute("pushaway", true),
                PropertiesName = boxnode.TryAttribute("properties", "Default")
            };
            return box;
        }
    }
}
