using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls
{
    public class LayoutObjectsLayer : ScreenLayer
    {
        private static BitmapImage StartIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/start_full.png"));
        private static BitmapImage ContinueIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/continue_full.png"));

        protected override void UnbindScreen(ScreenDocument oldScreen)
        {
            oldScreen.Stage.EntryPointsChanged -= Update;
        }

        protected override void BindScreen(ScreenDocument newScreen)
        {
            newScreen.Stage.EntryPointsChanged += Update;
        }

        protected override void Update()
        {
            InvalidateVisual();
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);

            if (Screen.Name == Screen.Stage.StartScreen)
            {
                var p = Screen.Stage.StartPoint;
                dc.DrawImage(StartIcon, new System.Windows.Rect(Zoom * (p.X - StartIcon.PixelWidth / 2), Zoom * (p.Y - StartIcon.PixelHeight / 2), Zoom * StartIcon.PixelWidth, Zoom * StartIcon.PixelHeight));
            }

            if (Screen.Stage.ContinuePoints.ContainsKey(Screen.Name))
            {
                var p = Screen.Stage.ContinuePoints[Screen.Name];
                dc.DrawImage(ContinueIcon, new System.Windows.Rect(Zoom * (p.X - ContinueIcon.PixelWidth / 2), Zoom * (p.Y - ContinueIcon.PixelHeight / 2), Zoom * ContinueIcon.PixelWidth, Zoom * ContinueIcon.PixelHeight));
            }
        }
    }
}
