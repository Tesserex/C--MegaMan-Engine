using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Effects
{
    internal class VarsEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Vars";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new VarsEffectPartInfo() {
                Name = partNode.GetAttribute<string>("name"),
                Value = partNode.GetAttribute<string>("value")
            };
        }
    }
}
