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
using MegaMan.Editor.Tools;

namespace MegaMan.Editor.Controls
{
    public class ScreenCanvas : Canvas
    {
        protected ScreenDocument _screen;

        protected TileScreenLayer _tiles;

        private IToolProvider _toolProvider;

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

                ScreenChanged();
            }
        }

        protected virtual void ScreenChanged() { }

        private void Resized(int width, int height)
        {
            Width = MaxWidth = MinWidth = _screen.PixelWidth;
            Height = MaxHeight = MinHeight = _screen.PixelHeight;
            InvalidateMeasure();
        }

        public ScreenCanvas(IToolProvider toolProvider)
        {
            _tiles = new TileScreenLayer();

            _toolProvider = toolProvider;

            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            VerticalAlignment = System.Windows.VerticalAlignment.Top;
            
            this.Children.Add(_tiles);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _tiles.Measure(new Size(_screen.PixelWidth, _screen.PixelHeight));
            return new Size(_screen.PixelWidth, _screen.PixelHeight);
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (_toolProvider.Tool == null)
            {
                return;
            }

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.Click(this.Screen, new Common.Geometry.Point((int)mousePoint.X, (int)mousePoint.Y));
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_toolProvider.Tool == null)
            {
                return;
            }

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.Release(this.Screen, new Common.Geometry.Point((int)mousePoint.X, (int)mousePoint.Y));
        }

        protected override void OnMouseRightButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (_toolProvider.Tool == null)
            {
                return;
            }

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.RightClick(this.Screen, new Common.Geometry.Point((int)mousePoint.X, (int)mousePoint.Y));
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_toolProvider.Tool == null)
            {
                return;
            }

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.Move(this.Screen, new Common.Geometry.Point((int)mousePoint.X, (int)mousePoint.Y));
        }

        static ScreenCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenCanvas), new FrameworkPropertyMetadata(typeof(ScreenCanvas)));
        }
    }
}
