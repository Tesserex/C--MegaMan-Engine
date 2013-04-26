using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MegaMan.Common;

using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XnaColor = Microsoft.Xna.Framework.Color;
using MegaMan.Engine.Rendering;
using MegaMan.Common.Geometry;
using Microsoft.Xna.Framework.Graphics;

namespace MegaMan.Engine
{
    public class XnaSpriteDrawer : ISpriteDrawer
    {
        private Sprite _info;

        private List<Texture2D> _paletteSwaps;

        private int? texture;

        private Palette palette;

        public XnaSpriteDrawer(Sprite info)
        {
            this._info = info;

            this._paletteSwaps = new List<Texture2D>();
        }

        public void SetTexture(GraphicsDevice device, string sheetPath)
        {
            VerifyPaletteSwaps(device);
        }

        public void DrawXna(IRenderingContext context, int layer, float positionX, float positionY)
        {
            if (!_info.Visible || _info.Count == 0 || context == null) return;

            if (texture == null)
                texture = context.LoadTexture(_info.SheetPath);

            /*
            SpriteEffects effect = SpriteEffects.None;
            if (_info.HorizontalFlip ^ _info.Reversed) effect = SpriteEffects.FlipHorizontally;
            if (_info.VerticalFlip) effect |= SpriteEffects.FlipVertically;
            */
            int hx = (_info.HorizontalFlip ^ _info.Reversed) ? _info.Width - _info.HotSpot.X : _info.HotSpot.X;
            int hy = _info.VerticalFlip ? _info.Height - _info.HotSpot.Y : _info.HotSpot.Y;

            // TODO: put back palette swaps
            var drawTexture = this.texture.Value;
            /*
            VerifyPaletteSwaps(batch.GraphicsDevice);
            if (this.palette != null && this._paletteSwaps.Count > this.palette.CurrentIndex)
            {
                drawTexture = this._paletteSwaps[this.palette.CurrentIndex];
            }
            */

            context.Draw(drawTexture, layer,
                new Common.Geometry.Point((int)(positionX - hx), (int)(positionY - hy)),
                new Common.Geometry.Rectangle(_info.CurrentFrame.SheetLocation.X, _info.CurrentFrame.SheetLocation.Y, _info.CurrentFrame.SheetLocation.Width, _info.CurrentFrame.SheetLocation.Height));
        }

        private void VerifyPaletteSwaps(GraphicsDevice device)
        {
            if (_info.PaletteName != null && this.palette == null)
            {
                this.palette = PaletteSystem.Get(_info.PaletteName) as Palette;
            }

            if (this.palette != null && this._paletteSwaps.Count == 0)
            {
                //this._paletteSwaps = this.palette.GenerateSwappedTextures((Bitmap)this.sheet, device);
            }
        }

        //private static Dictionary<string, Image> loadedImages = new Dictionary<string, Image>();
        /*
        private static Image GetImage(string absolutePath)
        {
            if (!loadedImages.ContainsKey(absolutePath))
            {
                loadedImages[absolutePath] = Image.FromFile(absolutePath);
            }

            return loadedImages[absolutePath];
        }
        */
        public static void Unload()
        {/*
            foreach (var image in loadedImages.Values)
            {
                image.Dispose();
            }

            loadedImages.Clear();*/
        }
    }
}
