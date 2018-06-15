using System;
using System.Xml;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Entities
{
    internal class SpriteComponentXmlWriter : IComponentXmlWriter
    {
        private readonly SpriteXmlWriter spriteWriter;

        public SpriteComponentXmlWriter(SpriteXmlWriter spriteWriter)
        {
            this.spriteWriter = spriteWriter;
        }

        public Type ComponentType { get { return typeof(SpriteComponentInfo); } }

        public void Write(IComponentInfo info, XmlWriter writer)
        {
            var spriteComponent = (SpriteComponentInfo)info;

            if (spriteComponent.SheetPath != null)
                writer.WriteElementString("Tilesheet", spriteComponent.SheetPath.Relative);

            foreach (var sprite in spriteComponent.Sprites.Values)
                spriteWriter.Write(sprite, writer);
        }
    }
}
