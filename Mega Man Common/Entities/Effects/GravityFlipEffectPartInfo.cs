using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class GravityFlipEffectPartInfo : IEffectPartInfo
    {
        public bool Flipped { get; set; }

        public IEffectPartInfo Clone()
        {
            return new GravityFlipEffectPartInfo() {
                Flipped = this.Flipped
            };
        }
    }
}
