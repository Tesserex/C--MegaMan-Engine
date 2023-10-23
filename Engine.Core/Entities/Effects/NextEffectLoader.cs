using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class NextEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(NextEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var next = (NextEffectPartInfo)info;
            
            return e => Game.CurrentGame.ProcessHandler(next.Transfer);
        }
    }
}
