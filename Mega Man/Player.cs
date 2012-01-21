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
        public GameEntity Entity { get; set; }
        public int PlayerLives { get; set; }

        public Player()
        {
            Entity = GameEntity.Get("Player");
            PlayerLives = 2;
        }

        public void ResetEntity()
        {
            Entity = GameEntity.Get("Player");

            Entity.Death += () => { PlayerLives--; };
        }

        internal void CollectItem(InventoryItems inventoryItems)
        {
            throw new NotImplementedException();
        }
    }
}
