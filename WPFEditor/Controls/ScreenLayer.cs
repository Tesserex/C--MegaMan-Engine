using System.Windows;
using System.Windows.Controls;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls
{
    public abstract class ScreenLayer : Canvas
    {
        private ScreenDocument _screen;

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
                    UnbindScreen(_screen);
                }

                _screen = value;

                if (_screen != null)
                {
                    BindScreen(_screen);
                }

                Update();
            }
        }

        protected double Zoom { get; private set; }

        protected abstract void UnbindScreen(ScreenDocument oldScreen);
        protected abstract void BindScreen(ScreenDocument newScreen);

        protected abstract void Update();

        public ScreenLayer()
        {
            ((App)App.Current).Tick += ScreenLayer_Tick;
            ViewModelMediator.Current.GetEvent<ZoomChangedEventArgs>().Subscribe(ZoomChanged);
            this.Zoom = 1;
        }

        private void ZoomChanged(object sender, ZoomChangedEventArgs e)
        {
            Zoom = e.Zoom;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(_screen.PixelWidth, _screen.PixelHeight);
        }

        private void ScreenLayer_Tick()
        {
            InvalidateVisual();
        }

        static ScreenLayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenLayer), new FrameworkPropertyMetadata(typeof(ScreenLayer)));
        }
    }
}
