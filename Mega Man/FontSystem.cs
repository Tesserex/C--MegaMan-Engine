using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Mega_Man
{
    public static class FontSystem
    {
        private class ImageFont : IDisposable
        {
            private int charWidth;
            private float charSpace;
            private Image charImg;
            private Texture2D charTex;

            private static List<char> chars = new List<char>(new char[] {
                                              'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
                                          });

            public ImageFont(Image img, Texture2D tex, int width, int space)
            {
                charImg = img;
                charTex = tex;
                charWidth = width;
                charSpace = space;
            }

            public void Draw(Graphics g, string text, PointF position)
            {
                text = text.ToLower();
                float xpos = position.X;

                foreach (char c in text)
                {
                    int cindex = chars.IndexOf(c);
                    if (cindex < 0) continue;

                    g.DrawImage(charImg, xpos, position.Y, new RectangleF(cindex * charWidth, 0, charWidth, charWidth), GraphicsUnit.Pixel);
                    
                    xpos += charWidth + charSpace;
                }
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

        private static Dictionary<string, ImageFont> fonts = new Dictionary<string,ImageFont>();

        public static void LoadFont(string name, string imagepath, int charWidth, int charSpace)
        {
            Image charimg = Image.FromFile(imagepath);
            Texture2D chartex = Texture2D.FromFile(Engine.Instance.GraphicsDevice, imagepath);
            ImageFont font = new ImageFont(charimg, chartex, charWidth, charSpace);

            fonts.Add(name, font);
        }

        public static void Draw(Graphics g, string font, string text, PointF position)
        {
            fonts[font].Draw(g, text, position);
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
