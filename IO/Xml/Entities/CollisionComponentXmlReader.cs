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
            var width = boxnode.GetAttribute<int>("width");
            var height = boxnode.GetAttribute<int>("height");
            var x = boxnode.GetAttribute<int>("x");
            var y = boxnode.GetAttribute<int>("y");

            var box = new HitBoxInfo {
                Name = boxnode.TryAttribute<string>("name"),
                Box = new Common.Geometry.Rectangle(x, y, width, height),
                ContactDamage = boxnode.TryAttribute<float>("damage"),
                Environment = boxnode.TryAttribute("environment", true),
                PushAway = boxnode.TryAttribute("pushaway", true),
                PropertiesName = boxnode.TryAttribute("properties", "Default")
            };
            return box;
        }
    }
}
