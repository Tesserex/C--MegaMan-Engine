using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return new CollisionEffectPartInfo() {
                ClearEnabled = this.ClearEnabled,
                Enabled = this.Enabled,
                EnabledBoxes = new List<string>(this.EnabledBoxes),
                HitBoxes = this.HitBoxes.Select(x => x.Clone()).ToList()
            };
        }
    }
}
