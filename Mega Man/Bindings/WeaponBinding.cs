using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MegaMan.Engine
{
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
