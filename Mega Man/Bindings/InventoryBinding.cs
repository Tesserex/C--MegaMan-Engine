using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MegaMan.Engine
{
    public class InventoryBinding : Binding
    {
        private string itemName;

        public InventoryBinding(object target, PropertyInfo targetProperty, string[] sourceParts)
            : base(target, targetProperty)
        {
            if (sourceParts.Length < 2)
            {
                throw new GameRunException(String.Format("Binding '{0}' is invalid. Inventory bindings must be of the form 'Inventory.ItemName'.", sourceParts[0]));
            }

            this.itemName = sourceParts[1];
        }

        public override void Start(IEntityContainer container)
        {
            var value = Game.CurrentGame.Player.ItemQuantity(itemName);
            Set(value);

            Game.CurrentGame.Player.InventoryChanged += Player_InventoryChanged;
        }

        public override void Stop()
        {
            Game.CurrentGame.Player.InventoryChanged -= Player_InventoryChanged;
        }

        private void Player_InventoryChanged(string itemName, int quantity)
        {
            if (itemName == this.itemName)
            {
                Set(quantity);
            }
        }

        private void Set(int value)
        {
            targetProperty.SetValue(target, value.ToString("00"), null);
        }
    }
}
