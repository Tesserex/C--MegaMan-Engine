using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Common.Entities
{
    public class StateComponentInfo
    {
        public List<StateInfo> States { get; private set; }

        public StateComponentInfo()
        {
            States = new List<StateInfo>();
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
