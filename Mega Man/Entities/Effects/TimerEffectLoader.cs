using System;
using System.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class TimerEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(TimerEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var timerInfo = (TimerEffectPartInfo)info;

            Effect effect = e => {
                e.CreateComponentIfNotExists<TimerComponent>();
            };

            effect = timerInfo.Start
                .Aggregate(effect, (current, timerName) => current + (entity => {
                    var name = timerName;
                    var timer = entity.GetComponent<TimerComponent>();
                    if (timer != null)
                        timer.Timers[name] = 0;
                }));

            effect = timerInfo.Reset
                .Aggregate(effect, (current, timerName) => current + (entity => {
                    var name = timerName;
                    var timer = entity.GetComponent<TimerComponent>();
                    if (timer != null && timer.Timers.ContainsKey(name))
                        timer.Timers[name] = 0;
                }));

            effect = timerInfo.Delete
                .Aggregate(effect, (current, timerName) => current + (entity => {
                    var timer = entity.GetComponent<TimerComponent>();
                    if (timer != null)
                        timer.Timers.Remove(timerName);
                }));

            return effect;
        }
    }
}
