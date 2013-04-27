using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MegaMan.Common;
using MegaMan.Engine.Rendering;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    public class XnaSpriteDrawer : ISpriteDrawer
    {
        private Sprite _info;

        private IResourceImage texture;

        public XnaSpriteDrawer(Sprite info)
        {
            this._info = info;
        }

        public void DrawXna(IRenderingContext context, int layer, float positionX, float positionY)
        {
            if (!_info.Visible || _info.Count == 0 || context == null) return;

            if (texture == null)
                texture = context.LoadResource(_info.SheetPath, _info.PaletteName);

            bool flipHorizontal = _info.HorizontalFlip ^ _info.Reversed;
            bool flipVertical = _info.VerticalFlip;

            int hx = (_info.HorizontalFlip ^ _info.Reversed) ? _info.Width - _info.HotSpot.X : _info.HotSpot.X;
            int hy = _info.VerticalFlip ? _info.Height - _info.HotSpot.Y : _info.HotSpot.Y;

            var drawTexture = this.texture;

            context.Draw(drawTexture, layer,
                new Common.Geometry.Point((int)(positionX - hx), (int)(positionY - hy)),
                new Common.Geometry.Rectangle(_info.CurrentFrame.SheetLocation.X, _info.CurrentFrame.SheetLocation.Y, _info.CurrentFrame.SheetLocation.Width, _info.CurrentFrame.SheetLocation.Height),
                flipHorizontal, flipVertical);
        }
    }
}
