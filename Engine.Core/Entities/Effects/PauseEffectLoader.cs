using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class PauseEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(PauseEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            return e => e.Paused = true;
        }
    }
}
