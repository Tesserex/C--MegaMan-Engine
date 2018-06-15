using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class AddInventoryEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(AddInventoryEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var inv = (AddInventoryEffectPartInfo)info;

            return e => Game.CurrentGame.Player.CollectItem(inv.ItemName, inv.Quantity);
        }
    }
}
