using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities
{
    public class CollisionComponentInfo
    {
        public bool Enabled { get; set; }
        public List<HitBoxInfo> HitBoxes { get; private set; }

        public CollisionComponentInfo()
        {
            HitBoxes = new List<HitBoxInfo>();
        }
    }
}
