using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class SoundEffectPartInfo : IEffectPartInfo
    {
        public string Name { get; set; }
        public bool Playing { get; set; }
    }
}
