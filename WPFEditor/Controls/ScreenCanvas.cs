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
        private ScreenDocument _screen;

        private TileScreenLayer _tiles;

        public ScreenDocument Screen
        {
            get
            {
                return _screen;
            }
            set
            {
                _screen = value;

                _tiles.Screen = value;
            }
        }

        public ScreenCanvas()
        {
            _tiles = new TileScreenLayer();
            
            this.Children.Add(_tiles);
        }

        static ScreenCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenCanvas), new FrameworkPropertyMetadata(typeof(ScreenCanvas)));
        }
    }
}
