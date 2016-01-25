using System.Collections.Generic;

namespace MegaMan.Common.Entities
{
    public class CollisionComponentInfo : IComponentInfo
    {
        public bool Enabled { get; set; }
        public List<HitBoxInfo> HitBoxes { get; private set; }

        public CollisionComponentInfo()
        {
            HitBoxes = new List<HitBoxInfo>();
        }
    }
}
