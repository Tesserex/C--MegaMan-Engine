using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Tools;
using Point = MegaMan.Common.Geometry.Point;

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
            Width = MaxWidth = MinWidth = _screen.PixelWidth * Zoom;
            Height = MaxHeight = MinHeight = _screen.PixelHeight * Zoom;
            InvalidateMeasure();
        }

        public ScreenCanvas(IToolProvider toolProvider)
        {
            _tiles = new TileScreenLayer();
            _guides = new GuidesLayer();
            _overlay = new OverlayScreenLayer();

            _toolProvider = toolProvider;

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            Children.Add(_tiles);
            Children.Add(_guides);
            Children.Add(_overlay);

            ViewModelMediator.Current.GetEvent<ZoomChangedEventArgs>().Subscribe(ZoomChanged);
        }

        public void Destroy()
        {
            _tiles.Destroy();
            _guides.Destroy();
            _overlay.Destroy();
            Children.Clear();
            ViewModelMediator.Current.GetEvent<ZoomChangedEventArgs>().Unsubscribe(ZoomChanged);
        }

        private void ZoomChanged(object sender, ZoomChangedEventArgs e)
        {
            Resized(_screen.Width, _screen.Height);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _tiles.Measure(new Size(_screen.PixelWidth * Zoom, _screen.PixelHeight * Zoom));
            return new Size(_screen.PixelWidth * Zoom, _screen.PixelHeight * Zoom);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (_toolProvider == null || _toolProvider.Tool == null)
                return;

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.Click(this, MouseLocation(mousePoint));
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_toolProvider == null || _toolProvider.Tool == null)
                return;

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.Release(this, MouseLocation(mousePoint));
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (_toolProvider == null || _toolProvider.Tool == null)
                return;

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.RightClick(this, MouseLocation(mousePoint));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_toolProvider == null || _toolProvider.Tool == null)
                return;

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.Move(this, MouseLocation(mousePoint));
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e) {
            if (_toolProvider.Tool != null && _toolProvider.Tool.SuppressContextMenu)
            {
                e.Handled = true;
            }

            base.OnContextMenuOpening(e);
        }

        private Point MouseLocation(System.Windows.Point mousePoint)
        {
            return new Point((int)(mousePoint.X / Zoom), (int)(mousePoint.Y / Zoom));
        }

        static ScreenCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenCanvas), new FrameworkPropertyMetadata(typeof(ScreenCanvas)));
        }
    }
}
