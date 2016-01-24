using System;
using System.Collections.Generic;
using MegaMan.Common;
using MegaMan.Common.Rendering;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    public static class FontSystem
    {
        private class ImageFont
        {
            private readonly FontInfo info;

            private IResourceImage charTex;

            public ImageFont(FontInfo info)
            {
                this.info = info;
            }

            public void Draw(IRenderingContext renderContext, int layer, string text, Point position)
            {
                if (charTex == null)
                    charTex = renderContext.LoadResource(info.ImagePath);

                if (!info.CaseSensitive)
                {
                    text = text.ToUpper();
                }

                int xpos = position.X;

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
                        renderContext.Draw(charTex, layer, new Point(xpos, position.Y),
                            new Rectangle(location.Value.X, location.Value.Y, info.CharWidth, info.CharWidth));

                        xpos += info.CharWidth;
                    }
                }
            }
        }

        private static readonly Dictionary<string, ImageFont> fonts = new Dictionary<string,ImageFont>();

        public static void Load(IEnumerable<FontInfo> fontInfos)
        {
            foreach (var fontInfo in fontInfos)
            {
                fonts.Add(fontInfo.Name, new ImageFont(fontInfo));
            }
        }

        public static void Draw(IRenderingContext renderContext, int layer, string font, string text, MegaMan.Common.Geometry.Point position)
        {
            fonts[font].Draw(renderContext, layer, text, position);
        }

        public static void Unload()
        {
            fonts.Clear();
        }
    }
}
