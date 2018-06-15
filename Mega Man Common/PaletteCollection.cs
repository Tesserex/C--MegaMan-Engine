using System;
using System.Collections;
using System.Collections.Generic;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.Common
{
    public class PaletteCollection : IEnumerable<PaletteInfo>
    {
        private readonly Dictionary<string, PaletteInfo> palettes = new Dictionary<string, PaletteInfo>();

        public void LoadPalettes(IEnumerable<PaletteInfo> addedPaletteInfos)
        {
            foreach (var info in addedPaletteInfos)
            {
                palettes.Add(info.Name, info);
            }
        }

        public PaletteInfo this[string name]
        {
            get
            {
                if (palettes.ContainsKey(name))
                    return palettes[name];
                return null;
            }
        }

        public int Count
        {
            get
            {
                return palettes.Count;
            }
        }

        IEnumerator<PaletteInfo> IEnumerable<PaletteInfo>.GetEnumerator()
        {
            return palettes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return palettes.Values.GetEnumerator();
        }
    }
}
