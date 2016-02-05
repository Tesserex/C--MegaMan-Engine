using System;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.IO.Xml.Handlers;

namespace MegaMan.IO.Xml.Effects
{
    internal class NextEffectPartXmlReader : IEffectPartXmlReader
    {
        private readonly HandlerTransferXmlReader _transferReader;

        public NextEffectPartXmlReader(HandlerTransferXmlReader transferReader)
        {
            _transferReader = transferReader;
        }

        public string NodeName
        {
            get
            {
                return "Next";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new NextEffectPartInfo() {
                Transfer = _transferReader.Load(partNode)
            };
        }
    }
}
