using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class EffectInfo
    {
        public string Name { get; set; }
        public IEnumerable<IEffectPartInfo> Parts { get; set; }
    }
}
