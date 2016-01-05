using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class SetVarEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(SetVarEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var setInfo = (SetVarEffectPartInfo)info;
            var name = setInfo.Name;
            var value = setInfo.Value;

            return e => Game.CurrentGame.Player.SetVar(name, value);
        }
    }
}
