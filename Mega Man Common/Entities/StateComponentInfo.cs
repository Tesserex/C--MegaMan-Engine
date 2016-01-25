using System.Collections.Generic;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Common.Entities
{
    public class StateComponentInfo : IComponentInfo
    {
        public List<StateInfo> States { get; private set; }
        public List<MultiStateTriggerInfo> Triggers { get; private set; }

        public StateComponentInfo()
        {
            States = new List<StateInfo>();
            Triggers = new List<MultiStateTriggerInfo>();
        }
    }

    public class StateInfo
    {
        public string Name { get; set; }

        public List<TriggerInfo> Triggers { get; private set; }
        public EffectInfo Initializer { get; set; }
        public EffectInfo Logic { get; set; }

        public StateInfo()
        {
            Triggers = new List<TriggerInfo>();
        }
    }
}
