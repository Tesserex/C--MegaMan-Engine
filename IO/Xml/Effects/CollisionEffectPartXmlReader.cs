using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common.Entities;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common.Geometry;

namespace MegaMan.IO.Xml.Effects
{
    internal class CollisionEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Collision";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            var info = new CollisionEffectPartInfo();

            var rects = new List<HitBoxInfo>();
            var enables = new HashSet<string>();

            info.ClearEnabled = partNode.Element("Clear") != null;

            var enable = partNode.Element("Enabled");
            if (enable != null)
            {
                info.Enabled = enable.GetValue<bool>();
            }

            foreach (var box in partNode.Elements("Hitbox"))
            {
                var boxinfo = new HitBoxInfo() {
                    Box = new Rectangle() {
                        X = box.GetAttribute<int>("x"),
                        Y = box.GetAttribute<int>("y"),
                        Width = box.GetAttribute<int>("width"),
                        Height = box.GetAttribute<int>("height")
                    },
                    ContactDamage = box.TryAttribute<float>("damage"),
                    Environment = box.TryAttribute("environment", true),
                    PushAway = box.TryAttribute("pushaway", true),
                    PropertiesName = box.TryAttribute("properties", "Default")
                };

                foreach (var groupnode in box.Elements("Hits"))
                    boxinfo.Hits.Add(groupnode.Value);

                foreach (var groupnode in box.Elements("Group"))
                    boxinfo.Groups.Add(groupnode.Value);

                foreach (var resistNode in box.Elements("Resist"))
                {
                    var resistName = resistNode.GetAttribute<string>("name");
                    var mult = resistNode.GetAttribute<float>("multiply");
                    boxinfo.Resistance.Add(resistName, mult);
                }

                rects.Add(boxinfo);
            }

            foreach (var enableBox in partNode.Elements("EnableBox"))
            {
                enables.Add(enableBox.GetAttribute<string>("name"));
            }

            info.HitBoxes = rects;
            info.EnabledBoxes = enables;

            return info;
        }
    }
}
