﻿using System;
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

            var condition = EffectParser.ParseCondition(triggerInfo.Trigger.Condition);
            var triggerEffect = EffectParser.LoadTriggerEffect(triggerInfo.Trigger.Effect);
            var elseEffect = (triggerInfo.Trigger.Else != null) ? EffectParser.LoadTriggerEffect(triggerInfo.Trigger.Else) : null;
            return e =>
            {
                if (condition(e)) triggerEffect(e);
                else elseEffect?.Invoke(e);
            };
        }
    }
}
