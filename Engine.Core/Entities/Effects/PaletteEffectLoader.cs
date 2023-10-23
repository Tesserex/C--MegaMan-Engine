using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class PaletteEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(PaletteEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var pal = (PaletteEffectPartInfo)info;

            return e =>
            {
                var palette = PaletteSystem.Get(pal.PaletteName);
                if (palette != null)
                {
                    palette.CurrentIndex = pal.PaletteIndex;
                }
            };
        }
    }
}
