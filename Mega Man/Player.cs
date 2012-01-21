using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine
{
    public enum InventoryItems
    {
        EnergyTank
    }

    public class Player
    {
        public int Lives { get; set; }

        public Player()
        {
            Lives = 2;
        }

        internal void CollectItem(InventoryItems inventoryItems)
        {
            throw new NotImplementedException();
        }
    }
}
