﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class RemoveEffectPartInfo : IEffectPartInfo
    {
        public IEffectPartInfo Clone()
        {
            return new RemoveEffectPartInfo();
        }
    }
}
