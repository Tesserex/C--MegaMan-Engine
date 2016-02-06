using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
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
