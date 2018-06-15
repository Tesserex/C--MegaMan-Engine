namespace MegaMan.Common.Entities.Effects
{
    public class WeaponEffectPartInfo : IEffectPartInfo
    {
        public string ChangeName { get; set; }
        public int? Ammo { get; set; }
        public WeaponAction Action { get; set; }

        public IEffectPartInfo Clone()
        {
            return new WeaponEffectPartInfo {
                ChangeName = ChangeName,
                Ammo = Ammo,
                Action = Action
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
