using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class RemoveEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Remove";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new RemoveEffectPartInfo();
        }
    }
}
