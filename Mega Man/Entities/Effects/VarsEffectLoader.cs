using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class VarsEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(VarsEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var varInfo = (VarsEffectPartInfo)info;

            return e => { e.GetComponent<VarsComponent>().Set(varInfo.Name, varInfo.Value); };
        }
    }
}
