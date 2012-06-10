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

            var component = player.GetComponent<WeaponComponent>();
            var value = component.Ammo(weaponName);
            var max = component.MaxAmmo(weaponName);
            Set(value / (float)max);

            player.GetComponent<WeaponComponent>().AmmoChanged += WeaponAmmo_Changed;
        }

        public override void Stop()
        {
            var player = container.GetEntities("Player").SingleOrDefault();
            if (player == null) return;
            player.GetComponent<WeaponComponent>().AmmoChanged -= WeaponAmmo_Changed;
        }

        private void WeaponAmmo_Changed(string weapon, int ammo, int max)
        {
            if (weapon == this.weaponName)
            {
                Set(ammo / (float)max);
            }
        }

        private void Set(float value)
        {
            targetProperty.SetValue(target, value, null);
        }
    }
}
