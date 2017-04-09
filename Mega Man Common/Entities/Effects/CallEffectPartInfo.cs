using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class CallEffectPartInfo : IEffectPartInfo
    {
        public string EffectName { get; set; }

        public IEffectPartInfo Clone()
        {
            return new CallEffectPartInfo() {
                EffectName = this.EffectName
            };
        }
    }
}
