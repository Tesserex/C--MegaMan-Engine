using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class CallEffectLoader : IEffectLoader
    {
        public Type PartInfoType { get { return typeof(CallEffectPartInfo); } }

        public Effect Load(IEffectPartInfo info)
        {
            var callInfo = (CallEffectPartInfo)info;
            return EffectParser.GetLateBoundEffect(callInfo.EffectName);
        }
    }
}
