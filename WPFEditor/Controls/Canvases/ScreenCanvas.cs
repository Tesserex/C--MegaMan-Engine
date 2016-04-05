using System;
using System.Windows;
using System.Windows.Controls;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Tools;

namespace MegaMan.Editor.Controls
{
    public class ScreenCanvas : Canvas
    {
        protected ScreenDocument _screen;

        protected TileScreenLayer _tiles;
        protected GuidesLayer _guides;
        protected OverlayScreenLayer _overlay;

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
                _guides.Screen = value;
                _overlay.Screen = value;

                _screen.Resized += Resized;

                ScreenChanged();
            }
        }

        public double Zoom { get { return Convert.ToDouble(App.Current.Resources["Zoom"] ?? 1); } }

        protected virtual void ScreenChanged()
        {
            Resized(_screen.Width, _screen.Height);
        }

        private void Resized(int width, int height)
        {
            Width = MaxWidth = MinWidth = _screen.PixelWidth * this.Zoom;
            Height = MaxHeight = MinHeight = _screen.PixelHeight * this.Zoom;
            InvalidateMeasure();
        }

        public ScreenCanvas(IToolProvider toolProvider)
        {
            _tiles = new TileScreenLayer();
            _guides = new GuidesLayer();
            _overlay = new OverlayScreenLayer();

            _toolProvider = toolProvider;

            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            VerticalAlignment = System.Windows.VerticalAlignment.Top;

            this.Children.Add(_tiles);
            this.Children.Add(_guides);
            this.Children.Add(_overlay);

            ViewModelMediator.Current.GetEvent<ZoomChangedEventArgs>().Subscribe(ZoomChanged);
        }

        private void ZoomChanged(object sender, ZoomChangedEventArgs e)
        {
            Resized(_screen.Width, _screen.Height);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _tiles.Measure(new Size(_screen.PixelWidth * this.Zoom, _screen.PixelHeight * this.Zoom));
            return new Size(_screen.PixelWidth * this.Zoom, _screen.PixelHeight * this.Zoom);
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (_toolProvider == null || _toolProvider.Tool == null)
                return;

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.Click(this, MouseLocation(mousePoint));
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_toolProvider == null || _toolProvider.Tool == null)
                return;

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.Release(this, MouseLocation(mousePoint));
        }

        protected override void OnMouseRightButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (_toolProvider == null || _toolProvider.Tool == null)
                return;

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.RightClick(this, MouseLocation(mousePoint));
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_toolProvider == null || _toolProvider.Tool == null)
                return;

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.Move(this, MouseLocation(mousePoint));
        }

        private Common.Geometry.Point MouseLocation(Point mousePoint)
        {
            return new Common.Geometry.Point((int)(mousePoint.X / this.Zoom), (int)(mousePoint.Y / this.Zoom));
        }

        static ScreenCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenCanvas), new FrameworkPropertyMetadata(typeof(ScreenCanvas)));
        }
    }
}
