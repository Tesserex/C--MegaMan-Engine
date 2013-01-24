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

        protected Rectangle _highlight;

        public ScreenDocument Screen
        {
            get
            {
                return _screen;
            }
            set
            {
                _screen = value;

                _highlight.Width = _screen.PixelWidth;
                _highlight.Height = _screen.PixelHeight;

                _tiles.Screen = value;
            }
        }

        public ScreenCanvas()
        {
            _tiles = new TileScreenLayer();

            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            VerticalAlignment = System.Windows.VerticalAlignment.Top;
            
            this.Children.Add(_tiles);

            _highlight = new Rectangle()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 5,
                Visibility = Visibility.Hidden,
            };

            this.Children.Add(_highlight);

            Canvas.SetZIndex(_highlight, 1000);
        }

        static ScreenCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenCanvas), new FrameworkPropertyMetadata(typeof(ScreenCanvas)));
        }
    }
}
