namespace MegaMan.Common.Entities.Effects
{
    public class DefeatBossEffectPartInfo : IEffectPartInfo
    {
        public string Name { get; set; }

        public IEffectPartInfo Clone()
        {
            return new DefeatBossEffectPartInfo {
                Name = Name
            };
        }
    }
}
