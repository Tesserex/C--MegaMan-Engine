using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine
{
    public class Player
    {
        public int Lives { get; set; }
        public event Action<int> LivesChanged;

        private Dictionary<string, string> _globalVars;
        private Dictionary<string, int> inventory;
        public event Action<string, int> InventoryChanged;

        private HashSet<string> unlockedWeapons;
        private HashSet<string> defeatedBosses;

        public Player()
        {
            Lives = 2;
            _globalVars = new Dictionary<string, string>();
            inventory = new Dictionary<string, int>();
            unlockedWeapons = new HashSet<string>();
            defeatedBosses = new HashSet<string>();
        }

        public string Var(string name)
        {
            return _globalVars.ContainsKey(name) ? _globalVars[name] : String.Empty;
        }

        public void SetVar(string name, string value)
        {
            _globalVars[name] = value;
        }

        public bool IsWeaponUnlocked(string name)
        {
            return unlockedWeapons.Contains(name);
        }

        public void UnlockWeapon(string name)
        {
            unlockedWeapons.Add(name);
        }

        public bool IsBossDefeated(string name)
        {
            return defeatedBosses.Contains(name);
        }

        public void DefeatBoss(string name)
        {
            defeatedBosses.Add(name);
        }

        public void CollectItem(string itemName, int quantity = 1)
        {
            if (!inventory.ContainsKey(itemName))
            {
                inventory[itemName] = 0;
            }

            inventory[itemName] += quantity;

            if (InventoryChanged != null) InventoryChanged(itemName, inventory[itemName]);
        }

        public bool UseItem(string itemName, int quantity = 1)
        {
            if (!inventory.ContainsKey(itemName) || inventory[itemName] < quantity)
            {
                return false;
            }

            inventory[itemName] -= quantity;

            if (InventoryChanged != null) InventoryChanged(itemName, inventory[itemName]);

            return true;
        }

        public int ItemQuantity(string itemName)
        {
            if (!inventory.ContainsKey(itemName))
            {
                return 0;
            }
            return inventory[itemName];
        }
    }
}
