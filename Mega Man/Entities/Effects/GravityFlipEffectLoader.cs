using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class GravityFlipEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(GravityFlipEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var grav = (GravityFlipEffectPartInfo)info;
            return entity => entity.Container.IsGravityFlipped = grav.Flipped;
        }
    }
}
