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
        public List<EffectInfo> Initializer { get; private set; }
        public List<EffectInfo> Logic { get; private set; }

        public StateInfo()
        {
            Triggers = new List<TriggerInfo>();
            Initializer = new List<EffectInfo>();
            Logic = new List<EffectInfo>();
        }
    }
}
