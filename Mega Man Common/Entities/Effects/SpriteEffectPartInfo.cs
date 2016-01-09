using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class SpriteEffectPartInfo : IEffectPartInfo
    {
        public string Name { get; set; }
        public bool? Playing { get; set; }
        public bool? Visible { get; set; }
    }
}
