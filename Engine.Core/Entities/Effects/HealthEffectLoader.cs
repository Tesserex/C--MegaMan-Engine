using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class HealthEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(HealthEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var health = (HealthEffectPartInfo)info;
            return entity =>
            {
                entity.GetComponent<HealthComponent>().Health += health.Change;
            };
        }
    }
}
