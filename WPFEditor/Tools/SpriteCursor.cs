using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Common;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Tools
{
    public class SpriteCursor : IToolCursor
    {
        private Sprite _sprite;

        private static ToolCursorAdorner _cursorAdorner;

        private FrameworkElement _element;

        public SpriteCursor(Sprite sprite)
        {
            _sprite = sprite;
        }

        private WriteableBitmap CursorImage
        {
            get { return SpriteBitmapCache.GetOrLoadFrame(_sprite.SheetPath.Absolute, _sprite.CurrentFrame.SheetLocation); }
        }

        public void ApplyCursorTo(FrameworkElement element)
        {
            _element = element;

            var layer = AdornerLayer.GetAdornerLayer(element);
            var hideCursor = (_cursorAdorner == null || _cursorAdorner.Visibility == Visibility.Hidden);

            if (_cursorAdorner != null)
                layer.Remove(_cursorAdorner);

            _cursorAdorner = new ToolCursorAdorner(element, Render);
            _cursorAdorner.SnapsToDevicePixels = true;
            _cursorAdorner.UseLayoutRounding = true;
            layer.Add(_cursorAdorner);

            WeakEventManager<FrameworkElement, MouseEventArgs>.AddHandler(element, "MouseEnter", MouseEnter);
            WeakEventManager<FrameworkElement, MouseEventArgs>.AddHandler(element, "MouseLeave", MouseLeave);

            element.Cursor = Cursors.None;
            if (hideCursor)
                _cursorAdorner.Visibility = Visibility.Hidden;
        }

        private void MouseEnter(object sender, MouseEventArgs args)
        {
            _cursorAdorner.Visibility = Visibility.Visible;
        }

        private void MouseLeave(object sender, MouseEventArgs args)
        {
            _cursorAdorner.Visibility = Visibility.Hidden;
        }

        public void Dispose()
        {
            if (_element != null)
            {
                WeakEventManager<FrameworkElement, MouseEventArgs>.RemoveHandler(_element, "MouseEnter", MouseEnter);
                WeakEventManager<FrameworkElement, MouseEventArgs>.RemoveHandler(_element, "MouseLeave", MouseLeave);
            }
        }

        private void Render(DrawingContext drawingContext)
        {
            var zoom = Convert.ToDouble(App.Current.Resources["Zoom"] ?? 1);

            var cursorPosition = Mouse.GetPosition(_element);

            var width = _sprite.Width * zoom;
            var height = _sprite.Height * zoom;

            var mx = cursorPosition.X - _sprite.HotSpot.X;
            var my = cursorPosition.Y - _sprite.HotSpot.Y;

            drawingContext.DrawImage(SpriteBitmapCache.Scale(this.CursorImage, zoom),
                new Rect(
                    mx,
                    my,
                    width,
                    height)
                );
        }
    }
}
