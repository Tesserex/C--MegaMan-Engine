namespace MegaMan.Common.Entities.Effects
{
    public class SoundEffectPartInfo : IEffectPartInfo
    {
        public string Name { get; set; }
        public bool Playing { get; set; }

        public IEffectPartInfo Clone()
        {
            return new SoundEffectPartInfo {
                Name = Name,
                Playing = Playing
            };
        }
    }
}
