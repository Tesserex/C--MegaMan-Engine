using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;
using MegaMan.IO.Xml.Handlers;

namespace MegaMan.IO.Xml.Effects
{
    internal class NextEffectPartXmlWriter : IEffectPartXmlWriter
    {
        private readonly HandlerTransferXmlWriter handlerWriter;

        public NextEffectPartXmlWriter(HandlerTransferXmlWriter handlerWriter)
        {
            this.handlerWriter = handlerWriter;
        }

        public Type EffectPartType
        {
            get
            {
                return typeof(NextEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            handlerWriter.Write(((NextEffectPartInfo)info).Transfer, writer);
        }
    }
}
