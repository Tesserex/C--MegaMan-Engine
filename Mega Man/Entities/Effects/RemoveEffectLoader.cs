using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class RemoveEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(RemoveEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            return e => e.Remove();
        }
    }
}
