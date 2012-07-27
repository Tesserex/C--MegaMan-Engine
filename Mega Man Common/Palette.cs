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

        public static void LoadPalettes<T>(XElement parentNode, string basePath) where T : Palette, new()
        {
            foreach (var node in parentNode.Elements("Palette"))
            {
                var palette = FromXml<T>(node, basePath);

                palettes.Add(palette.Name, palette);
            }
        }

        public static Palette Get(string paletteName)
        {
            if (palettes.ContainsKey(paletteName))
            {
                return palettes[paletteName];
            }

            return null;
        }

        public static Palette FromXml<T>(XElement node, string basePath) where T : Palette, new()
        {
            var imagePathRelative = node.RequireAttribute("image").Value;
            var imagePath = FilePath.FromRelative(imagePathRelative, basePath);
            var name = node.RequireAttribute("name").Value;

            var palette = new T();
            palette.Initialize(name, imagePath);
            return palette;
        }

        public static void Unload()
        {
            palettes.Clear();
        }

        private List<Dictionary<uint, uint>> _swapColors;

        public string Name { get; private set; }

        public int CurrentIndex { get; set; }

        public abstract void Initialize(string name, FilePath imagePath);
    }
}
