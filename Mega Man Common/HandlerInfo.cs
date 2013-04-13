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
        public Sprite Sprite { get; set; }

        public string Name { get { return Sprite.Name; } }

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

        public virtual void Save(XmlTextWriter writer)
        {
            foreach (var obj in Objects.Values)
            {
                obj.Save(writer);
            }
        }
    }
}
