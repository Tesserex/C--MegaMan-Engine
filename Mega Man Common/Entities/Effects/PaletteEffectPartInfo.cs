namespace MegaMan.Common.Entities.Effects
{
    public class PaletteEffectPartInfo : IEffectPartInfo
    {
        public string PaletteName { get; set; }
        public int PaletteIndex { get; set; }

        public IEffectPartInfo Clone()
        {
            return new PaletteEffectPartInfo {
                PaletteIndex = PaletteIndex,
                PaletteName = PaletteName
            };
        }
    }
}
