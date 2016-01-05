using System;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Effects
{
    internal class UnpauseEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Unpause";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new UnpauseEffectPartInfo();
        }
    }
}
