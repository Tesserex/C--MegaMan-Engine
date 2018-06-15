using System;
using System.Xml;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO.Xml.Handlers
{
    internal class HandlerSpriteXmlWriter : IHandlerObjectXmlWriter
    {
        private readonly SpriteXmlWriter spriteWriter;

        public HandlerSpriteXmlWriter(SpriteXmlWriter spriteWriter)
        {
            this.spriteWriter = spriteWriter;
        }

        public void Write(IHandlerObjectInfo info, XmlWriter writer)
        {
            var spr = (HandlerSpriteInfo)info;
            spriteWriter.Write(spr.Sprite, writer);
        }

        public Type ObjectType
        {
            get { return typeof(HandlerSpriteInfo); }
        }
    }
}
