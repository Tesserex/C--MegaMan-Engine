using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls
{
    public class ScreenCanvas : Canvas
    {
        protected ScreenDocument _screen;

        protected TileScreenLayer _tiles;

        public ScreenDocument Screen
        {
            get
            {
                return _screen;
            }
            set
            {
                if (_screen != null)
                {
                    _screen.Resized -= Resized;
                }

                _screen = value;

                _tiles.Screen = value;

                _screen.Resized += Resized;
            }
        }

        private void Resized(int width, int height)
        {
            Width = MaxWidth = MinWidth = _screen.PixelWidth;
            Height = MaxHeight = MinHeight = _screen.PixelHeight;
            InvalidateMeasure();
        }

        public ScreenCanvas()
        {
            _tiles = new TileScreenLayer();

            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            VerticalAlignment = System.Windows.VerticalAlignment.Top;
            
            this.Children.Add(_tiles);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _tiles.Measure(new Size(_screen.PixelWidth, _screen.PixelHeight));
            return new Size(_screen.PixelWidth, _screen.PixelHeight);
        }

        static ScreenCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenCanvas), new FrameworkPropertyMetadata(typeof(ScreenCanvas)));
        }
    }
}
