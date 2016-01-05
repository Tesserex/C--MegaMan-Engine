using System;
using System.Xml.Linq;
using System.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Effects
{
    internal class NextEffectPartXmlReader : IEffectPartXmlReader
    {
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
                Transfer = GameXmlReader.LoadHandlerTransfer(partNode)
            };
        }
    }
}
