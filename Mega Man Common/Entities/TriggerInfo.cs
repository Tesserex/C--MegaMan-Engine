using System;
using System.Collections.Generic;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Common.Entities
{
    public class TriggerInfo
    {
        public string Condition { get; set; }
        public EffectInfo Effect { get; set; }
        public EffectInfo Else { get; set; }
        public int? Priority { get; set; }

        public TriggerInfo Clone()
        {
            return new TriggerInfo() {
                Condition = this.Condition,
                Effect = this.Effect.Clone(),
                Else = this.Else.Clone(),
                Priority = this.Priority
            };
        }
    }

    public class MultiStateTriggerInfo
    {
        public List<string> States { get; set; }
        public TriggerInfo Trigger { get; set; }
    }
}
