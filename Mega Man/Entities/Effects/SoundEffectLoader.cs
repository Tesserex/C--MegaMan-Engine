using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class SoundEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(SoundEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var sound = (SoundEffectPartInfo)info;

            return entity => {
                entity.CreateComponentIfNotExists<SoundComponent>();
                var msg = new SoundMessage(entity, sound.Name, sound.Playing);
                entity.SendMessage(msg);
            };
        }
    }
}
