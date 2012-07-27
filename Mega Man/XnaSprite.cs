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
    public class XnaSprite : Sprite
    {
        private List<Texture2D> _paletteSwaps;

        private Texture2D texture;

        private Image sheet;

        public Image Sheet { get { return this.sheet; } }

        private XnaPalette palette;

        public override FilePath SheetPath
        {
            get
            {
                return base.SheetPath;
            }
            set
            {
                base.SheetPath = value;
                this.sheet = Image.FromFile(value.Absolute);
            }
        }

        public override void Initialize(int width, int height)
        {
            base.Initialize(width, height);

            this._paletteSwaps = new List<Texture2D>();

            this.sheet = null;
        }

        public XnaSprite(XnaSprite copy)
            : base(copy)
        {
            this._paletteSwaps = copy._paletteSwaps;
            this.texture = copy.texture;
            this.sheet = copy.sheet;
        }

        protected override SpriteFrame CreateFrame()
        {
            return new SpriteFrame(this, this.sheet, 0, DrawRectangle.Empty);
        }

        public void SetTexture(GraphicsDevice device, string sheetPath)
        {
            StreamReader sr = new StreamReader(sheetPath);
            this.texture = Texture2D.FromStream(device, sr.BaseStream);

            if (this.sheet == null)
            {
                this.sheet = Image.FromFile(sheetPath);
            }

            VerifyPaletteSwaps(device);
        }

        public void DrawXna(SpriteBatch batch, XnaColor color, float positionX, float positionY)
        {
            if (!Visible || this.Count == 0 || batch == null || this.texture == null) return;

            SpriteEffects effect = SpriteEffects.None;
            if (HorizontalFlip ^ this.Reversed) effect = SpriteEffects.FlipHorizontally;
            if (VerticalFlip) effect |= SpriteEffects.FlipVertically;

            int hx = (HorizontalFlip ^ this.Reversed) ? this.Width - this.HotSpot.X : this.HotSpot.X;
            int hy = VerticalFlip ? this.Height - this.HotSpot.Y : this.HotSpot.Y;

            // check palette swap
            var drawTexture = this.texture;
            VerifyPaletteSwaps(batch.GraphicsDevice);
            if (this.palette != null && this._paletteSwaps.Count > this.palette.CurrentIndex)
            {
                drawTexture = this._paletteSwaps[this.palette.CurrentIndex];
            }

            batch.Draw(drawTexture,
                new XnaRectangle((int)(positionX),
                    (int)(positionY), this.Width, this.Height),
                new XnaRectangle(this[currentFrame].SheetLocation.X, this[currentFrame].SheetLocation.Y, this[currentFrame].SheetLocation.Width, this[currentFrame].SheetLocation.Height),
                color, 0,
                new Vector2(hx, hy), effect, 0);
        }

        private void VerifyPaletteSwaps(GraphicsDevice device)
        {
            if (PaletteName != null && this.palette == null)
            {
                this.palette = Palette.Get(PaletteName);
            }

            if (this.palette != null && this._paletteSwaps.Count == 0)
            {
                this._paletteSwaps = this.palette.GenerateSwappedTextures((Bitmap)this.sheet, device);
            }
        }
    }
}
