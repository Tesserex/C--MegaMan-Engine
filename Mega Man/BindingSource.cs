using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public abstract class Binding
    {
        protected object target;
        protected PropertyInfo targetProperty;

        public static Binding Create(SceneBindingInfo info, object target)
        {
            var sourceParts = info.Source.Split('.');
            if (sourceParts.Length == 0)
            {
                throw new GameRunException(String.Format("Binding source '{0}' is invalid.", info.Source));
            }

            var targetProperty = target.GetType().GetProperty(info.Target);

            if (targetProperty == null)
            {
                throw new GameRunException(String.Format("Binding target '{0}' is invalid.", info.Target));
            }

            switch (sourceParts[0].ToUpper())
            {
                case "INVENTORY":
                    return new InventoryBinding(target, targetProperty, sourceParts);
                
                case "WEAPON":
                    return new WeaponBinding(target, targetProperty, sourceParts[1]);
                
                default:
                    throw new GameRunException(String.Format("Binding '{0}' is invalid.", info.Source));
            }
        }

        protected Binding(object target, PropertyInfo targetProperty)
        {
            this.target = target;
            this.targetProperty = targetProperty;
        }

        public abstract void Start(IEntityContainer container);
        public abstract void Stop();
    }

    public class InventoryBinding : Binding
    {
        private string itemName;

        public InventoryBinding(object target, PropertyInfo targetProperty, string[] sourceParts) : base(target, targetProperty)
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

    public class WeaponBinding : Binding
    {
        private string weaponName;
        private IEntityContainer container;

        public WeaponBinding(object target, PropertyInfo targetProperty, string weaponName)
            : base(target, targetProperty)
        {
            this.weaponName = weaponName;
        }

        public override void Start(IEntityContainer container)
        {
            this.container = container;

            var player = container.GetEntities("Player").SingleOrDefault();
            if (player == null) return;

            var value = player.GetComponent<WeaponComponent>().Ammo(weaponName);
            Set(value);

            player.GetComponent<WeaponComponent>().AmmoChanged += WeaponAmmo_Changed;
        }

        public override void Stop()
        {
            var player = container.GetEntities("Player").SingleOrDefault();
            if (player == null) return;
            player.GetComponent<WeaponComponent>().AmmoChanged -= WeaponAmmo_Changed;
        }

        private void WeaponAmmo_Changed(string weapon, int ammo)
        {
            if (weapon == this.weaponName)
            {
                Set(ammo);
            }
        }

        private void Set(int value)
        {
            targetProperty.SetValue(target, value, null);
        }
    }
}
