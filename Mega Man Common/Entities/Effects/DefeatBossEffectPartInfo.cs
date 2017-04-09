using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class DefeatBossEffectPartInfo : IEffectPartInfo
    {
        public string Name { get; set; }

        public IEffectPartInfo Clone()
        {
            return new DefeatBossEffectPartInfo() {
                Name = this.Name
            };
        }
    }
}
