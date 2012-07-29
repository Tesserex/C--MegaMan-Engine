using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MegaMan.Common;

using DrawPoint = System.Drawing.Point;
using DrawRectangle = System.Drawing.Rectangle;

using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace MegaMan.Engine
{
    public class XnaSpriteDrawer : ISpriteDrawer
    {
        private Sprite _info;

        private List<Texture2D> _paletteSwaps;

        private Texture2D texture;

        private Image sheet;

        private XnaPalette palette;

        public XnaSpriteDrawer(Sprite info)
        {
            this._info = info;

            this._paletteSwaps = new List<Texture2D>();

            this.sheet = null;
        }

        public void SetTexture(GraphicsDevice device, string sheetPath)
        {
            StreamReader sr = new StreamReader(sheetPath);
            this.texture = Texture2D.FromStream(device, sr.BaseStream);

            if (this.sheet == null)
            {
                this.sheet = GetImage(sheetPath);
            }

            VerifyPaletteSwaps(device);
        }

        public void DrawXna(SpriteBatch batch, XnaColor color, float positionX, float positionY)
        {
            if (!_info.Visible || _info.Count == 0 || batch == null || this.texture == null) return;

            SpriteEffects effect = SpriteEffects.None;
            if (_info.HorizontalFlip ^ _info.Reversed) effect = SpriteEffects.FlipHorizontally;
            if (_info.VerticalFlip) effect |= SpriteEffects.FlipVertically;

            int hx = (_info.HorizontalFlip ^ _info.Reversed) ? _info.Width - _info.HotSpot.X : _info.HotSpot.X;
            int hy = _info.VerticalFlip ? _info.Height - _info.HotSpot.Y : _info.HotSpot.Y;

            // check palette swap
            var drawTexture = this.texture;
            VerifyPaletteSwaps(batch.GraphicsDevice);
            if (this.palette != null && this._paletteSwaps.Count > this.palette.CurrentIndex)
            {
                drawTexture = this._paletteSwaps[this.palette.CurrentIndex];
            }

            batch.Draw(drawTexture,
                new XnaRectangle((int)(positionX),
                    (int)(positionY), _info.Width, _info.Height),
                new XnaRectangle(_info.CurrentFrame.SheetLocation.X, _info.CurrentFrame.SheetLocation.Y, _info.CurrentFrame.SheetLocation.Width, _info.CurrentFrame.SheetLocation.Height),
                color, 0,
                new Vector2(hx, hy), effect, 0);
        }

        private void VerifyPaletteSwaps(GraphicsDevice device)
        {
            if (_info.PaletteName != null && this.palette == null)
            {
                this.palette = Palette.Get(_info.PaletteName) as XnaPalette;
            }

            if (this.palette != null && this._paletteSwaps.Count == 0)
            {
                this._paletteSwaps = this.palette.GenerateSwappedTextures((Bitmap)this.sheet, device);
            }
        }

        private static Dictionary<string, Image> loadedImages = new Dictionary<string, Image>();

        private static Image GetImage(string absolutePath)
        {
            if (!loadedImages.ContainsKey(absolutePath))
            {
                loadedImages[absolutePath] = Image.FromFile(absolutePath);
            }

            return loadedImages[absolutePath];
        }

        public static void Unload()
        {
            foreach (var image in loadedImages.Values)
            {
                image.Dispose();
            }

            loadedImages.Clear();
        }
    }
}
