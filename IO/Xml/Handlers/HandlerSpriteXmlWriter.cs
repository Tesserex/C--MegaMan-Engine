using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers
{
    internal class HandlerSpriteXmlWriter : IHandlerObjectXmlWriter
    {
        private readonly SpriteXmlWriter _spriteWriter;

        public HandlerSpriteXmlWriter(SpriteXmlWriter spriteWriter)
        {
            _spriteWriter = spriteWriter;
        }

        public void Write(IHandlerObjectInfo info, XmlWriter writer)
        {
            var spr = (HandlerSpriteInfo)info;
            _spriteWriter.Write(spr.Sprite, writer);
        }

        public Type ObjectType
        {
            get { return typeof(HandlerSpriteInfo); }
        }
    }
}
