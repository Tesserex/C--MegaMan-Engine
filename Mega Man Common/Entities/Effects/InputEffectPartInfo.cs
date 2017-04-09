using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class InputEffectPartInfo : IEffectPartInfo
    {
        public bool Paused { get; set; }

        public IEffectPartInfo Clone()
        {
            return new InputEffectPartInfo() {
                Paused = this.Paused
            };
        }
    }
}
