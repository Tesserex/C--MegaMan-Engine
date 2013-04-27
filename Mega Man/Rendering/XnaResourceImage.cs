using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Rendering
{
    public class XnaResourceImage : IResourceImage
    {
        public int ResourceId
        {
            get;
            private set;
        }

        public string PaletteName
        {
            get;
            private set;
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public XnaResourceImage(int id, string palette, int width, int height)
        {
            ResourceId = id;
            PaletteName = palette;
            Width = width;
            Height = height;
        }
    }
}
