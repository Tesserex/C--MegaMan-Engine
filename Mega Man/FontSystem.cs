using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.IO.Xml;

namespace MegaMan.Engine
{
    public static class FontSystem
    {
        private class ImageFont : IDisposable
        {
            private readonly FontInfo info;

            private Image charImg;
            private readonly Texture2D charTex;

            public ImageFont(FontInfo info)
            {
                this.info = info;

                charImg = Image.FromFile(info.ImagePath.Absolute);
                StreamReader sr = new StreamReader(info.ImagePath.Absolute);
                charTex = Texture2D.FromStream(Engine.Instance.GraphicsDevice, sr.BaseStream);
            }

            public void Draw(SpriteBatch batch, string text, Vector2 position)
            {
                if (!info.CaseSensitive)
                {
                    text = text.ToUpper();
                }

                float xpos = position.X;

                foreach (char c in text)
                {
                    if (c == ' ')
                    {
                        xpos += info.CharWidth;
                        continue;
                    }

                    var location = info[c];

                    if (location != null)
                    {
                        batch.Draw(charTex, new Vector2(xpos, position.Y),
                            new Microsoft.Xna.Framework.Rectangle(location.Value.X, location.Value.Y, info.CharWidth, info.CharWidth), Engine.Instance.OpacityColor);

                        xpos += info.CharWidth;
                    }
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

                var fontInfo = IncludeFileXmlReader.LoadFont(fontNode, Game.CurrentGame.BasePath);
                fonts.Add(name, new ImageFont(fontInfo));
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
