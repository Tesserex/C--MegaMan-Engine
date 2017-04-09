using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class LivesEffectPartInfo : IEffectPartInfo
    {
        public int Add { get; set; }

        public IEffectPartInfo Clone()
        {
            return new LivesEffectPartInfo() {
                Add = this.Add
            };
        }
    }
}
