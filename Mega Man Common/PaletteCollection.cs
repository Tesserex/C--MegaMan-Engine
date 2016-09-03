using System;
using System.Collections.Generic;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.Common
{
    public class PaletteCollection : IEnumerable<PaletteInfo>
    {
        private Dictionary<string, PaletteInfo> _palettes = new Dictionary<string, PaletteInfo>();

        public void LoadPalettes(IEnumerable<PaletteInfo> palettes)
        {
            foreach (var info in palettes)
            {
                _palettes.Add(info.Name, info);
            }
        }

        public PaletteInfo this[string name]
        {
            get
            {
                if (_palettes.ContainsKey(name))
                    return _palettes[name];
                else
                    return null;
            }
        }

        public Int32 Count
        {
            get
            {
                return _palettes.Count;
            }
        }

        IEnumerator<PaletteInfo> IEnumerable<PaletteInfo>.GetEnumerator()
        {
            return _palettes.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _palettes.Values.GetEnumerator();
        }
    }
}
