using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class PauseEffectPartInfo : IEffectPartInfo
    {
        public IEffectPartInfo Clone()
        {
            return new PauseEffectPartInfo();
        }
    }
}
