using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class DieEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(DieEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            return e => e.Die();
        }
    }
}
