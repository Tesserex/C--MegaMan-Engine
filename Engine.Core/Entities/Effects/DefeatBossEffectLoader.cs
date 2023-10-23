using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class DefeatBossEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(DefeatBossEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var boss = (DefeatBossEffectPartInfo)info;
            return e => Game.CurrentGame.Player.DefeatBoss(boss.Name);
        }
    }
}
