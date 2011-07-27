using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace MegaMan.Engine
{
    public static class FontSystem
    {
        private class ImageFont : IDisposable
        {
            private readonly int charWidth;
            private readonly float charSpace;
            private Image charImg;
            private readonly Texture2D charTex;

            private static readonly List<char> chars = new List<char>(new[] {
                                              'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '.', ',', '!', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
                                          });

            public ImageFont(Image img, Texture2D tex, int width, int space)
            {
                charImg = img;
                charTex = tex;
                charWidth = width;
                charSpace = space;
            }

            public void Draw(SpriteBatch batch, string text, Vector2 position)
            {
                text = text.ToLower();
                float xpos = position.X;

                foreach (char c in text)
                {
                    int cindex = chars.IndexOf(c);
                    if (cindex < 0) continue;

                    batch.Draw(charTex, new Vector2(xpos, position.Y), new Microsoft.Xna.Framework.Rectangle(cindex * charWidth, 0, charWidth, charWidth), Engine.Instance.OpacityColor);

                    xpos += charWidth + charSpace;
                }
            }

            #region IDisposable Members

            ~ImageFont()
            {
                Dispose();
            }

            public void Dispose()
            {
                if (charImg != null)
                {
                    charImg.Dispose();
                    charImg = null;
                }
            }

            #endregion
        }

        private static readonly Dictionary<string, ImageFont> fonts = new Dictionary<string,ImageFont>();

        public static void LoadFont(string name, string imagepath, int charWidth, int charSpace)
        {
            Image charimg = Image.FromFile(imagepath);
            StreamReader sr = new StreamReader(imagepath);
            Texture2D chartex = Texture2D.FromStream(Engine.Instance.GraphicsDevice, sr.BaseStream);
            ImageFont font = new ImageFont(charimg, chartex, charWidth, charSpace);

            fonts.Add(name, font);
        }

        public static void Draw(SpriteBatch batch, string font, string text, PointF position)
        {
            fonts[font].Draw(batch, text, new Vector2(position.X, position.Y));
        }

        public static void Unload()
        {
            foreach (ImageFont font in fonts.Values)
            {
                font.Dispose();
            }
            fonts.Clear();
        }
    }
}
