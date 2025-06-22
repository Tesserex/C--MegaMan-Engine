using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Core.Entities.Effects
{
    public class LivesEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(LivesEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var lives = (LivesEffectPartInfo)info;
            return e => Game.CurrentGame.Player.Lives += lives.Add;
        }
    }
}
