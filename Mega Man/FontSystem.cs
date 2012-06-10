using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public static class FontSystem
    {
        private class ImageFont : IDisposable
        {
            private readonly int charWidth;
            private readonly bool caseSensitive;
            private Image charImg;
            private readonly Texture2D charTex;
            private readonly Dictionary<char, System.Drawing.Point> chars;

            public ImageFont(XElement node)
            {
                charWidth = node.GetInteger("charwidth");
                caseSensitive = node.GetBool("cased");

                chars = new Dictionary<char, System.Drawing.Point>();

                foreach (var lineNode in node.Elements("Line"))
                {
                    var x = lineNode.GetInteger("x");
                    var y = lineNode.GetInteger("y");

                    var lineText = lineNode.Value;

                    if (!caseSensitive)
                    {
                        lineText = lineText.ToUpper();
                    }

                    var lineChars = lineText.ToCharArray();

                    for (int i = 0; i < lineChars.Length; i++)
                    {
                        var c = lineChars[i];

                        chars.Add(c, new System.Drawing.Point(x + i * charWidth, y));
                    }
                }

                var imagepath = System.IO.Path.Combine(Game.CurrentGame.BasePath, node.RequireAttribute("image").Value);
                charImg = Image.FromFile(imagepath);
                StreamReader sr = new StreamReader(imagepath);
                charTex = Texture2D.FromStream(Engine.Instance.GraphicsDevice, sr.BaseStream);
            }

            public void Draw(SpriteBatch batch, string text, Vector2 position)
            {
                if (!caseSensitive)
                {
                    text = text.ToUpper();
                }

                float xpos = position.X;

                foreach (char c in text)
                {
                    if (c == ' ')
                    {
                        xpos += charWidth;
                        continue;
                    }

                    if (!chars.ContainsKey(c)) continue;

                    var location = chars[c];

                    batch.Draw(charTex, new Vector2(xpos, position.Y), new Microsoft.Xna.Framework.Rectangle(location.X, location.Y, charWidth, charWidth), Engine.Instance.OpacityColor);

                    xpos += charWidth;
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

        public static void Load(XElement node)
        {
            foreach (var fontNode in node.Elements("Font"))
            {
                var name = fontNode.RequireAttribute("name").Value;

                fonts.Add(name, new ImageFont(fontNode));
            }
        }

        public static void Draw(SpriteBatch batch, string font, string text, PointF position)
        {
            fonts[font].Draw(batch, text, new Vector2(position.X, position.Y));
        }

        public static void Draw(SpriteBatch batch, string font, string text, System.Drawing.Point position)
        {
            fonts[font].Draw(batch, text, new Vector2(position.X, position.Y));
        }

        public static void Draw(SpriteBatch batch, string font, string text, Microsoft.Xna.Framework.Point position)
        {
            fonts[font].Draw(batch, text, new Vector2(position.X, position.Y));
        }

        public static void Draw(SpriteBatch batch, string font, string text, Vector2 position)
        {
            fonts[font].Draw(batch, text, position);
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
