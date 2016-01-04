using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class DieEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Die";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new DieEffectPartInfo();
        }
    }
}
