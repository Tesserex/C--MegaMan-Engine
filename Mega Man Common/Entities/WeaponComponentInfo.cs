using System.Collections.Generic;

namespace MegaMan.Common.Entities
{
    public class WeaponComponentInfo : IComponentInfo
    {
        public List<WeaponInfo> Weapons { get; set; }
    }

    public class WeaponInfo
    {
        public string Name { get; set; }
        public string EntityName { get; set; }
        public int? Ammo { get; set; }
        public int? Usage { get; set; }
        public int? Palette { get; set; }
        public MeterInfo Meter { get; set; }
    }
}
