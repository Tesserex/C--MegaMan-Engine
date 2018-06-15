using System.Windows;
using System.Windows.Media;
using MegaMan.Common;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls
{
    public class LayoutObjectsLayer : ScreenLayer
    {
        private Sprite _playerSprite;

        protected override void UnbindScreen(ScreenDocument oldScreen)
        {
            oldScreen.Stage.EntryPointsChanged -= Update;
        }

        protected override void BindScreen(ScreenDocument newScreen)
        {
            newScreen.Stage.EntryPointsChanged += Update;

            var player = newScreen.Stage.Project.EntityByName("Player");
            if (player != null)
                _playerSprite = player.DefaultSprite;
        }

        protected override void Update()
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var image = SpriteBitmapCache.GetOrLoadFrame(_playerSprite.SheetPath.Absolute, _playerSprite[0].SheetLocation);
            image = SpriteBitmapCache.Scale(image, Zoom);

            if (Screen.Name == Screen.Stage.StartScreen)
            {
                var p = Screen.Stage.StartPoint;
                dc.DrawImage(image, new Rect(Zoom * (p.X - _playerSprite.HotSpot.X), Zoom * (p.Y - _playerSprite.HotSpot.Y), image.PixelWidth, image.PixelHeight));
            }

            if (Screen.Stage.ContinuePoints.ContainsKey(Screen.Name))
            {
                var p = Screen.Stage.ContinuePoints[Screen.Name];
                dc.DrawImage(image, new Rect(Zoom * (p.X - _playerSprite.HotSpot.X), Zoom * (p.Y - _playerSprite.HotSpot.Y), image.PixelWidth, image.PixelHeight));
            }
        }
    }
}
