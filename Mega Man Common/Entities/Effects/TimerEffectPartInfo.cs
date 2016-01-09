using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class TimerEffectPartInfo : IEffectPartInfo
    {
        public List<string> Start { get; set; }
        public List<string> Reset { get; set; }
        public List<string> Delete { get; set; }
    }
}
