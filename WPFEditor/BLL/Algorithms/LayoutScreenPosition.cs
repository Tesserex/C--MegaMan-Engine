using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Editor.Bll.Algorithms
{
    public class LayoutScreenPosition
    {
        public ScreenDocument Screen { get; private set; }

        public Point Location { get; set; }

        public LayoutScreenPosition(ScreenDocument screen, Point location)
        {
            Screen = screen;
            Location = location;
        }

        public int Left
        {
            get
            {
                return Location.X;
            }
        }

        public int Top
        {
            get
            {
                return Location.Y;
            }
        }

        public int Right
        {
            get
            {
                return Location.X + Screen.Width;
            }
        }

        public int Bottom
        {
            get
            {
                return Location.Y + Screen.Height;
            }
        }

        public Rectangle GetLocation(Point offset)
        {
            return new Rectangle(Location.X + offset.X, Location.Y + offset.Y, Screen.Width, Screen.Height);
        }
    }
}
