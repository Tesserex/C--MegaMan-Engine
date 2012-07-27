using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace MegaMan.Engine
{
    public class XnaPalette : Palette
    {
        public void Initialize(string name, FilePath imagePath)
        {
            this.Name = name;

            using (var img = (Bitmap)Image.FromFile(imagePath.Absolute))
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

        public List<Texture2D> GenerateSwappedTextures(Bitmap image, GraphicsDevice device)
        {
            var swappedPixels = GetSwappedPixels(image, true);

            var swappedTextures = new List<Texture2D>();

            var imageRect = new Rectangle(0, 0, image.Width, image.Height);

            foreach (var pixels in swappedPixels)
            {
                var texture = new Texture2D(device, image.Width, image.Height);

                texture.SetData<byte>(pixels);

                swappedTextures.Add(texture);
            }

            return swappedTextures;
        }

        private List<byte[]> GetSwappedPixels(Bitmap image, bool flip_endian)
        {
            var imageRect = new Rectangle(0, 0, image.Width, image.Height);

            var data = image.LockBits(imageRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelData = new Byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, pixelData, 0, data.Height * data.Stride);

            // Create buffers for pixel data in other palettes
            List<byte[]> swappedPixels = new List<byte[]>();
            for (int i = 0; i < _swapColors.Count; i++)
            {
                swappedPixels.Add(new byte[pixelData.Length]);
            }

            // fill all palette buffers simultaneously
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                uint key = (uint)((pixelData[i] << 24) + (pixelData[i + 1] << 16) + (pixelData[i + 2] << 8) + (pixelData[i + 3]));

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

            image.UnlockBits(data);

            return swappedPixels;
        }
    }
}
