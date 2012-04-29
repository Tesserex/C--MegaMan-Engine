using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public interface IHandlerObject
    {
        string Name { get; }
        void Save(XmlTextWriter writer);
    }

    public class HandlerSprite : IHandlerObject
    {
        public Sprite Sprite { get; private set; }

        public string Name { get { return Sprite.Name; } }

        public static HandlerSprite FromXml(XElement node, string basePath)
        {
            var info = new HandlerSprite();
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
        public Dictionary<string, IHandlerObject> Objects { get; private set; }

        public HandlerInfo()
        {
            this.Objects = new Dictionary<string, IHandlerObject>();
        }

        protected virtual void Load(XElement node, string basePath)
        {
            this.Name = node.RequireAttribute("name").Value;

            foreach (var spriteNode in node.Elements("Sprite"))
            {
                var sprite = HandlerSprite.FromXml(spriteNode, basePath);
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
