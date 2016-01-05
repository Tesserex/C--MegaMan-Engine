using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class TriggerEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(TriggerEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var triggerInfo = (TriggerEffectPartInfo)info;

            Condition condition = EffectParser.ParseCondition(triggerInfo.Trigger.Condition);
            Effect triggerEffect = EffectParser.LoadTriggerEffect(triggerInfo.Trigger.Effect);
            return e =>
            {
                if (condition(e)) triggerEffect(e);
            };
        }
    }
}
