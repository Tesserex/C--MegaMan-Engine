using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class AddInventoryEffectPartInfo : IEffectPartInfo
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }

        public IEffectPartInfo Clone()
        {
            return new AddInventoryEffectPartInfo() {
                ItemName = this.ItemName,
                Quantity = this.Quantity
            };
        }
    }
}
