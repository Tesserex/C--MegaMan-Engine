using System.Windows.Media;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls
{
    public class GuidesLayer : ScreenLayer
    {
        private static readonly Pen InnerPen = new Pen(Brushes.LightGray, 1);
        private static readonly Pen OuterPen = new Pen(Brushes.DarkSlateGray, 1);

        private bool _borderVisible;

        public GuidesLayer()
            : base()
        {
            ViewModelMediator.Current.GetEvent<LayerVisibilityChangedEventArgs>().Subscribe(LayerVisibilityChanged);
        }

        private void LayerVisibilityChanged(object sender, LayerVisibilityChangedEventArgs e)
        {
            _borderVisible = e.BordersVisible;
        }

        protected override void UnbindScreen(ScreenDocument oldScreen)
        {
        }

        protected override void BindScreen(ScreenDocument newScreen)
        {
        }

        protected override void Update()
        {
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_borderVisible)
            {
                dc.DrawRectangle(null, OuterPen, new System.Windows.Rect(0.5, 0.5, Zoom * Screen.PixelWidth - 1, Zoom * Screen.PixelHeight - 1));
                dc.DrawRectangle(null, InnerPen, new System.Windows.Rect(1.5, 1.5, Zoom * Screen.PixelWidth - 3, Zoom * Screen.PixelHeight - 3));
            }
        }
    }
}
