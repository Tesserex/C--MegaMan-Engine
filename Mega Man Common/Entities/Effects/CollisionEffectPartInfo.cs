using System.Collections.Generic;
using System.Linq;

namespace MegaMan.Common.Entities.Effects
{
    public class CollisionEffectPartInfo : IEffectPartInfo
    {
        public bool ClearEnabled { get; set; }
        public bool? Enabled { get; set; }
        public IEnumerable<string> EnabledBoxes { get; set; }
        public IEnumerable<HitBoxInfo> HitBoxes { get; set; }

        public IEffectPartInfo Clone()
        {
            return new CollisionEffectPartInfo {
                ClearEnabled = ClearEnabled,
                Enabled = Enabled,
                EnabledBoxes = new List<string>(EnabledBoxes),
                HitBoxes = HitBoxes.Select(x => x.Clone()).ToList()
            };
        }
    }
}
