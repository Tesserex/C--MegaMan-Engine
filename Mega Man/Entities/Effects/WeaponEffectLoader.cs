using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class WeaponEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(WeaponEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var wpn = (WeaponEffectPartInfo)info;

            switch (wpn.Action)
            {
                case WeaponAction.Shoot:
                    return entity =>
                    {
                        WeaponComponent weaponComponent = entity.GetComponent<WeaponComponent>();
                        if (weaponComponent != null) weaponComponent.Shoot();
                    };

                case WeaponAction.RotateForward:
                    return entity =>
                    {
                        WeaponComponent weaponComponent = entity.GetComponent<WeaponComponent>();
                        if (weaponComponent != null) weaponComponent.RotateForward();
                    };

                case WeaponAction.RotateBackward:
                    return entity => {
                        WeaponComponent weaponComponent = entity.GetComponent<WeaponComponent>();
                        if (weaponComponent != null) weaponComponent.RotateBackward();
                    };

                case WeaponAction.Change:
                    var weaponName = wpn.ChangeName;
                    return entity => {
                        WeaponComponent weaponComponent = entity.GetComponent<WeaponComponent>();
                        if (weaponComponent != null) weaponComponent.SetWeapon(weaponName);
                    };

                case WeaponAction.Ammo:
                    if (wpn.Ammo == null)
                        throw new GameRunException("Weapon effect of type Ammo did not specify a value.");

                    int val = wpn.Ammo.Value;
                    return entity => {
                        WeaponComponent weaponComponent = entity.GetComponent<WeaponComponent>();
                        if (weaponComponent != null) weaponComponent.AddAmmo(val);
                    };

                default:
                    throw new GameRunException("Weapon effect did not specify a valid type");
            }
        }
    }
}
