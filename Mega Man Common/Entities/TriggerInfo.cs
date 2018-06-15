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
            return new TriggerInfo {
                Condition = Condition,
                Effect = Effect.Clone(),
                Else = Else.Clone(),
                Priority = Priority
            };
        }
    }

    public class MultiStateTriggerInfo
    {
        public List<string> States { get; set; }
        public TriggerInfo Trigger { get; set; }
    }
}
