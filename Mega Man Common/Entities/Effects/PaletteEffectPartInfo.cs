using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class PaletteEffectPartInfo : IEffectPartInfo
    {
        public string PaletteName { get; set; }
        public int PaletteIndex { get; set; }
    }
}
