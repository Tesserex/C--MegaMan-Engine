namespace MegaMan.Common.Entities.Effects
{
    public class UnlockWeaponEffectPartInfo : IEffectPartInfo
    {
        public string WeaponName { get; set; }

        public IEffectPartInfo Clone()
        {
            return new UnlockWeaponEffectPartInfo {
                WeaponName = WeaponName
            };
        }
    }
}
