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
    }
}
