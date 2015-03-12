using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Tools
{
    public abstract class ImageCursor : IToolCursor
    {
        private static ToolCursorAdorner _cursorAdorner;

        private FrameworkElement _element;

        private static Pen outlinePen = new Pen(new SolidColorBrush(Colors.Silver) { Opacity = 0.5 }, 2);
        private static Pen shadowPen = new Pen(new SolidColorBrush(Colors.Black) { Opacity = 0.5 }, 2);

        protected ImageCursor()
        {
        }

        protected abstract ImageSource CursorImage
        {
            get;
        }

        public void ApplyCursorTo(FrameworkElement element)
        {
            _element = element;

            var layer = AdornerLayer.GetAdornerLayer(element);
            var hideCursor = (_cursorAdorner == null || _cursorAdorner.Visibility == Visibility.Hidden);

            if (_cursorAdorner != null)
                layer.Remove(_cursorAdorner);

            _cursorAdorner = new ToolCursorAdorner(element, Render);
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

        protected abstract float Width { get; }
        protected abstract float Height { get; }
        protected virtual float SnapWidth { get { return 1; } }
        protected virtual float SnapHeight { get { return 1; } }

        private void Render(DrawingContext drawingContext)
        {
            var zoom = Convert.ToDouble(App.Current.Resources["Zoom"] ?? 1);

            var cursorPosition = Mouse.GetPosition(_element);

            var width = this.Width * zoom;
            var height = this.Height * zoom;
            var snapWidth = this.SnapWidth * zoom;
            var snapHeight = this.SnapHeight * zoom;

            var snapX = (int)(cursorPosition.X / snapWidth) * snapWidth;
            var snapY = (int)(cursorPosition.Y / snapHeight) * snapHeight;

            drawingContext.DrawImage(this.CursorImage,
                new Rect(
                    snapX,
                    snapY,
                    width,
                    height)
                );

            drawingContext.DrawRectangle(null, outlinePen, new Rect(snapX, snapY, width, height));
            drawingContext.DrawRoundedRectangle(null, shadowPen, new Rect(snapX - 1, snapY - 1, width + 2, height + 2), 2, 2);
        }
    }
}
