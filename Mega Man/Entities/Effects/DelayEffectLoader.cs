using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class DelayEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(DelayedEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var delayInfo = (DelayedEffectPartInfo)info;
            var frames = delayInfo.DelayFrames;
            var effect = EffectParser.LoadTriggerEffect(delayInfo.Effect);
            return e =>
            {
                Engine.Instance.DelayedCall(() => effect(e), null, frames);
            };
        }
    }
}
