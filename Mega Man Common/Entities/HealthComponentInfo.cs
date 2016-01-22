using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities
{
    public class HealthComponentInfo
    {
        public float Max { get; set; }
        public float? StartValue { get; set; }
        public int FlashFrames { get; set; }

        public MeterInfo Meter { get; set; }
    }
}
