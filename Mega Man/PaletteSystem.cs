﻿using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.Engine
{
    public class PaletteSystem
    {
        private static Dictionary<string, Palette> palettes = new Dictionary<string, Palette>();

        public static void LoadPalettes(IEnumerable<PaletteInfo> paletteInfos)
        {
            foreach (var info in paletteInfos)
            {
                palettes.Add(info.Name, new Palette(info));
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
    }

    public class Palette
    {
        private PaletteInfo _info;
        private List<Dictionary<uint, uint>> _swapColors;

        public int CurrentIndex { get; set; }

        public Palette(PaletteInfo info)
        {
            this._info = info;
            Initialize();
        }

        private void Initialize()
        {
            using (var img = (Bitmap)Image.FromFile(_info.ImagePath.Absolute))
            {
                var imageRect = new Rectangle(0, 0, img.Width, img.Height);

                var paletteData = img.LockBits(imageRect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                byte[] paletteBytes = new byte[paletteData.Height * paletteData.Stride];

                try
                {
                    Marshal.Copy(paletteData.Scan0, paletteBytes, 0, paletteData.Height * paletteData.Stride);
                }
                finally
                {
                    img.UnlockBits(paletteData);
                }

                _swapColors = new List<Dictionary<uint, uint>>();

                var s = paletteData.Stride;

                for (int line = 0; line < paletteData.Height; line++)
                {
                    var pal = new Dictionary<uint, uint>();

                    for (int i = 0; i < s; i += 4)
                    {
                        var swap_i = i + (line * s);

                        uint key = (uint)((paletteBytes[i] << 24) + (paletteBytes[i + 1] << 16) + (paletteBytes[i + 2] << 8) + (paletteBytes[i + 3]));
                        uint value = (uint)((paletteBytes[swap_i] << 24) + (paletteBytes[swap_i + 1] << 16) + (paletteBytes[swap_i + 2] << 8) + (paletteBytes[swap_i + 3]));

                        pal[key] = value;
                    }

                    _swapColors.Add(pal);
                }
            }
        }

        public List<byte[]> GetSwappedPixels(byte[] pixelData, bool flip_endian)
        {
            List<byte[]> swappedPixels = new List<byte[]>();
            for (int i = 0; i < _swapColors.Count; i++)
            {
                swappedPixels.Add(new byte[pixelData.Length]);
            }

            // fill all palette buffers simultaneously
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                uint key;
                
                if (flip_endian)
                    key = (uint)((pixelData[i + 2] << 24) + (pixelData[i + 1] << 16) + (pixelData[i] << 8) + (pixelData[i + 3]));
                else
                    key = (uint)((pixelData[i] << 24) + (pixelData[i + 1] << 16) + (pixelData[i + 2] << 8) + (pixelData[i + 3]));

                for (int swap_i = 0; swap_i < swappedPixels.Count; swap_i++)
                {
                    var palette = _swapColors[swap_i];

                    uint value = key;

                    if (palette.ContainsKey(key))
                    {
                        value = palette[key];
                    }

                    if (flip_endian)
                    {
                        swappedPixels[swap_i][i + 2] = (byte)((value & (255 << 24)) >> 24);
                        swappedPixels[swap_i][i + 1] = (byte)((value & (255 << 16)) >> 16);
                        swappedPixels[swap_i][i] = (byte)((value & (255 << 8)) >> 8);
                        swappedPixels[swap_i][i + 3] = (byte)(value & 255);
                    }
                    else
                    {
                        swappedPixels[swap_i][i] = (byte)((value & (255 << 24)) >> 24);
                        swappedPixels[swap_i][i + 1] = (byte)((value & (255 << 16)) >> 16);
                        swappedPixels[swap_i][i + 2] = (byte)((value & (255 << 8)) >> 8);
                        swappedPixels[swap_i][i + 3] = (byte)(value & 255);
                    }
                }
            }

            return swappedPixels;
        }
    }
}
