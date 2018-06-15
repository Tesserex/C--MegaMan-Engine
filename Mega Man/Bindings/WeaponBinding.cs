﻿using System.Reflection;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    public class WeaponBinding : Binding
    {
        private string weaponName;
        private IEntityPool _entityPool;

        public WeaponBinding(object target, PropertyInfo targetProperty, string weaponName)
            : base(target, targetProperty)
        {
            this.weaponName = weaponName;
        }

        public override void Start(IEntityPool entityPool)
        {
            _entityPool = entityPool;

            var player = entityPool.GetEntityById("Player");
            if (player == null) return;

            var component = player.GetComponent<WeaponComponent>();
            var value = component.Ammo(weaponName);
            var max = component.MaxAmmo(weaponName);
            Set(value / (float)max);

            player.GetComponent<WeaponComponent>().AmmoChanged += WeaponAmmo_Changed;
        }

        public override void Stop()
        {
            var player = _entityPool.GetEntityById("Player");
            if (player == null) return;
            player.GetComponent<WeaponComponent>().AmmoChanged -= WeaponAmmo_Changed;
        }

        private void WeaponAmmo_Changed(string weapon, int ammo, int max)
        {
            if (weapon == weaponName)
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
