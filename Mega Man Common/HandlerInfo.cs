using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public interface IHandlerObjectInfo
    {
        string Name { get; }
        void Save(XmlTextWriter writer);
    }

    public class HandlerSpriteInfo : IHandlerObjectInfo
    {
        public Sprite Sprite { get; private set; }

        public string Name { get { return Sprite.Name; } }

        public static HandlerSpriteInfo FromXml(XElement node, string basePath)
        {
            var info = new HandlerSpriteInfo();
            info.Sprite = Sprite.FromXml(node, basePath);
            return info;
        }

        public void Save(XmlTextWriter writer)
        {
            Sprite.WriteTo(writer);
        }
    }

    public abstract class HandlerInfo
    {
        public string Name { get; set; }
        public Dictionary<string, IHandlerObjectInfo> Objects { get; private set; }

        public HandlerInfo()
        {
            this.Objects = new Dictionary<string, IHandlerObjectInfo>();
        }

        protected virtual void Load(XElement node, string basePath)
        {
            this.Name = node.RequireAttribute("name").Value;

            foreach (var spriteNode in node.Elements("Sprite"))
            {
                var sprite = HandlerSpriteInfo.FromXml(spriteNode, basePath);
                this.Objects.Add(sprite.Name, sprite);
            }

            foreach (var meterNode in node.Elements("Meter"))
            {
                var meter = MeterInfo.FromXml(meterNode, basePath);
                this.Objects.Add(meter.Name, meter);
            }
        }

        public virtual void Save(XmlTextWriter writer)
        {
            foreach (var obj in Objects.Values)
            {
                obj.Save(writer);
            }
        }
    }
}
