﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class TriggerEffectPartInfo : IEffectPartInfo
    {
        public TriggerInfo Trigger { get; set; }

        public IEffectPartInfo Clone()
        {
            return new TriggerEffectPartInfo() {
                Trigger = this.Trigger.Clone()
            };
        }
    }
}
