using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Editor.Bll.Algorithms
{
    public class ConnectedScreenSegment
    {
        private List<LayoutScreenPosition> _screens = new List<LayoutScreenPosition>();

        private int? _area;

        public Point Location { get; set; }

        public Boolean Placed { get; set; }

        public IEnumerable<LayoutScreenPosition> Screens
        {
            get
            {
                return _screens.AsReadOnly();
            }
        }

        public IEnumerable<String> ScreenNames
        {
            get
            {
                return _screens.Select(s => s.Screen.Name);
            }
        }

        public int Area
        {
            get
            {
                if (_area == null)
                {
                    _area = Width * Height;
                }

                return _area.Value;
            }
        }

        public int Width
        {
            get
            {
                return _screens.Max(s => s.Right);
            }
        }

        public int Height
        {
            get
            {
                return _screens.Max(s => s.Bottom);
            }
        }

        public bool CollidesWidth(ConnectedScreenSegment otherSegment)
        {
            var myRect = new Rectangle(Location.X, Location.Y, Width, Height);
            var otherRect = new Rectangle(otherSegment.Location.X, otherSegment.Location.Y, otherSegment.Width, otherSegment.Height);

            if (Rectangle.Intersect(myRect, otherRect) == Rectangle.Empty)
            {
                return false;
            }

            return _screens.Any(s => otherSegment._screens.Any(o => Rectangle.Intersect(s.GetLocation(this.Location), o.GetLocation(otherSegment.Location)) != Rectangle.Empty));
        }

        public void GrowLayout(ScreenDocument screen, IEnumerable<ScreenDocument> allScreens)
        {
            _area = null;

            GrowLayout(screen, allScreens, Point.Empty);

            NormalizePositions();
        }

        private void GrowLayout(ScreenDocument screen, IEnumerable<ScreenDocument> allScreens, Point location)
        {
            var screenPosition = new LayoutScreenPosition(screen, location);
            // add this screen
            _screens.Add(screenPosition);

            // use its joins to branch out
            foreach (var join in screen.Joins)
            {
                if (!ScreenNames.Contains(join.screenOne))
                {
                    var screenOne = allScreens.Single(s => s.Name == join.screenOne);

                    if (join.type == Common.JoinType.Horizontal)
                    {
                        Point offset = new Point(join.offsetTwo - join.offsetOne, -screenOne.Height);
                        GrowLayout(screenOne, allScreens, new Point(location.X + offset.X, location.Y + offset.Y));
                    }
                    else
                    {
                        Point offset = new Point(-screenOne.Width, join.offsetTwo - join.offsetOne);
                        GrowLayout(screenOne, allScreens, new Point(location.X + offset.X, location.Y + offset.Y));
                    }
                }

                if (!ScreenNames.Contains(join.screenTwo))
                {
                    var screenTwo = allScreens.Single(s => s.Name == join.screenTwo);

                    if (join.type == Common.JoinType.Horizontal)
                    {
                        Point offset = new Point(join.offsetOne - join.offsetTwo, screen.Height);
                        GrowLayout(screenTwo, allScreens, new Point(location.X + offset.X, location.Y + offset.Y));
                    }
                    else
                    {
                        Point offset = new Point(screen.Width, join.offsetOne - join.offsetTwo);
                        GrowLayout(screenTwo, allScreens, new Point(location.X + offset.X, location.Y + offset.Y));
                    }
                }
            }
        }

        private void NormalizePositions()
        {
            var minX = _screens.Min(s => s.Location.X);
            var minY = _screens.Min(s => s.Location.Y);

            foreach (var screen in _screens)
            {
                screen.Location = new Point(screen.Location.X - minX, screen.Location.Y - minY);
            }
        }
    }
}
