using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common.Geometry;

namespace MegaMan.IO.Xml.Effects
{
    public class CollisionEffectPartXmlReader : IEffectPartXmlReader
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
            HashSet<string> enables = new HashSet<string>();

            info.ClearEnabled = partNode.Element("Clear") != null;

            var enable = partNode.Element("Enabled");
            if (enable != null)
            {
                info.Enabled = enable.GetValue<bool>();
            }

            foreach (var box in partNode.Elements("Hitbox"))
            {
                var boxinfo = new HitBoxInfo() {
                    Box = new RectangleF() {
                        X = box.GetAttribute<float>("x"),
                        Y = box.GetAttribute<float>("y"),
                        Width = box.GetAttribute<float>("width"),
                        Height = box.GetAttribute<float>("height")
                    },
                    ContactDamage = box.TryAttribute<float>("damage"),
                    Environment = box.TryAttribute<bool>("environment", true),
                    PushAway = box.TryAttribute<bool>("pushaway", true),
                    PropertiesName = box.TryAttribute<string>("properties", "Default")
                };

                foreach (var groupnode in box.Elements("Hits"))
                    boxinfo.Hits.Add(groupnode.Value);

                foreach (var groupnode in box.Elements("Group"))
                    boxinfo.Groups.Add(groupnode.Value);

                foreach (var resistNode in box.Elements("Resist"))
                {
                    var resistName = resistNode.GetAttribute<string>("name");
                    float mult = resistNode.GetAttribute<float>("multiply");
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
