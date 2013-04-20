using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Runtime.InteropServices;

namespace MegaMan.Common
{
    public abstract class Palette
    {
        private static Dictionary<string, Palette> palettes = new Dictionary<string, Palette>();

        public static void Add(string name, Palette palette)
        {
            palettes.Add(name, palette);
        }

        public static Palette Get(string paletteName)
        {
            if (palettes.ContainsKey(paletteName))
            {
                return palettes[paletteName];
            }

            return null;
        }

        public static void ResetAll()
        {
            foreach (var palette in palettes.Values)
            {
                palette.CurrentIndex = 0;
            }
        }

        public static void Unload()
        {
            palettes.Clear();
        }

        public string Name { get; protected set; }

        public int CurrentIndex { get; set; }

        public abstract void Initialize(string name, FilePath imagePath);
    }
}
