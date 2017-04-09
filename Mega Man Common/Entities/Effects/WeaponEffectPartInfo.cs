using System;

namespace MegaMan.Common.Entities.Effects
{
    public class WeaponEffectPartInfo : IEffectPartInfo
    {
        public string ChangeName { get; set; }
        public int? Ammo { get; set; }
        public WeaponAction Action { get; set; }

        public IEffectPartInfo Clone()
        {
            return new WeaponEffectPartInfo() {
                ChangeName = this.ChangeName,
                Ammo = this.Ammo,
                Action = this.Action
            };
        }
    }

    public enum WeaponAction
    {
        Shoot,
        RotateForward,
        RotateBackward,
        Ammo,
        Change
    }
}
