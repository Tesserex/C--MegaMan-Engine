using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class UnlockWeaponEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(UnlockWeaponEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var wpn = (UnlockWeaponEffectPartInfo)info;
            return e => Game.CurrentGame.Player.UnlockWeapon(wpn.WeaponName);
        }
    }
}
