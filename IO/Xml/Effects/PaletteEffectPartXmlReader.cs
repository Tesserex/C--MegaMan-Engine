using System;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Effects
{
    internal class PaletteEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Palette";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new PaletteEffectPartInfo() {
                PaletteName = partNode.GetAttribute<string>("name"),
                PaletteIndex = partNode.GetAttribute<int>("index")
            };
        }
    }
}
