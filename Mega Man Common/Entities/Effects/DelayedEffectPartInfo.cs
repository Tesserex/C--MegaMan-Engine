using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class DelayedEffectPartInfo : IEffectPartInfo
    {
        public int DelayFrames { get; set; }
        public EffectInfo Effect { get; set; }

        public IEffectPartInfo Clone()
        {
            return new DelayedEffectPartInfo() {
                DelayFrames = this.DelayFrames,
                Effect = this.Effect.Clone()
            };
        }
    }
}
