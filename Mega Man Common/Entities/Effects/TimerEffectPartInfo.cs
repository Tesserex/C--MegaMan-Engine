using System.Collections.Generic;

namespace MegaMan.Common.Entities.Effects
{
    public class TimerEffectPartInfo : IEffectPartInfo
    {
        public List<string> Start { get; set; }
        public List<string> Reset { get; set; }
        public List<string> Delete { get; set; }

        public IEffectPartInfo Clone()
        {
            return new TimerEffectPartInfo {
                Start = new List<string>(Start),
                Reset = new List<string>(Reset),
                Delete = new List<string>(Delete)
            };
        }
    }
}
