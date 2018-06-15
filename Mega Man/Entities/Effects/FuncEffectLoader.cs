using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class FuncEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(FuncEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var func = (FuncEffectPartInfo)info;

            Effect effect = entity => { };
            foreach (var st in func.Statements)
            {
                effect += EffectParser.CompileEffect(st);
            }

            return effect;
        }
    }
}
