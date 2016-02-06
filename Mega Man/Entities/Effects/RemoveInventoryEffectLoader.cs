using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class RemoveInventoryEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(RemoveInventoryEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var inv = (RemoveInventoryEffectPartInfo)info;

            return e => Game.CurrentGame.Player.UseItem(inv.ItemName, inv.Quantity);
        }
    }
}
