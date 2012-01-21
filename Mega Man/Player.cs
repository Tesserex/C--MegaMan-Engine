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
        private GamePlay gameplay;

        public GameEntity Entity { get; set; }
        public int PlayerLives { get; set; }

        public Player(GamePlay game)
        {
            this.gameplay = game;
            Entity = GameEntity.Get("Player", game);
            PlayerLives = 2;
        }

        public void ResetEntity()
        {
            Entity = GameEntity.Get("Player", gameplay);

            Entity.Death += () => { PlayerLives--; };
        }

        internal void CollectItem(InventoryItems inventoryItems)
        {
            throw new NotImplementedException();
        }
    }
}
