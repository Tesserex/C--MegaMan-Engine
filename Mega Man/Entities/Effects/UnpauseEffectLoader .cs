using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class UnpauseEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(UnpauseEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            return e => e.Paused = false;
        }
    }
}
