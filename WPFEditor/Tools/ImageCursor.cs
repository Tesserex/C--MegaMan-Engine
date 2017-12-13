using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Tools
{
    public abstract class ImageCursor : IToolCursor
    {
        private static ToolCursorAdorner _cursorAdorner;
        private static Cursor _dotCursor;

        private FrameworkElement _element;
        private int _hotX = 0, _hotY = 0;

        private static Pen outlinePen = new Pen(new SolidColorBrush(Colors.Silver) { Opacity = 0.5 }, 2);
        private static Pen shadowPen = new Pen(new SolidColorBrush(Colors.Black) { Opacity = 0.5 }, 2);

        static ImageCursor()
        {
            var stream = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/dot.cur"));
            _dotCursor = new Cursor(stream.Stream);
            
        }

        protected ImageCursor(Common.Geometry.Point? hotspot = null)
        {
            if (hotspot.HasValue)
            {
                _hotX = hotspot.Value.X;
                _hotY = hotspot.Value.Y;
            }

            DrawOutline = true;
        }

        protected abstract ImageSource CursorImage
        {
            get;
        }

        protected bool DrawOutline { get; set; }

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

            element.Cursor = _dotCursor;
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

            var scrollX = 0d;
            var scrollY = 0d;
            if (_element is ScrollViewer)
            {
                scrollX = ((ScrollViewer)_element).HorizontalOffset;
                scrollY = ((ScrollViewer)_element).VerticalOffset;
            }

            var cursorPosition = Mouse.GetPosition(_element);

            var width = this.Width * zoom;
            var height = this.Height * zoom;
            var snapWidth = this.SnapWidth * zoom;
            var snapHeight = this.SnapHeight * zoom;
            var scrollOffsetX = (int)scrollX % snapWidth;
            var scrollOffsetY = (int)scrollY % snapHeight;

            var snapX = (int)((cursorPosition.X + scrollOffsetX) / snapWidth) * snapWidth - scrollOffsetX;
            var snapY = (int)((cursorPosition.Y + scrollOffsetY) / snapHeight) * snapHeight - scrollOffsetY;

            var finalX = snapX - (_hotX * zoom);
            var finalY = snapY - (_hotY * zoom);

            drawingContext.DrawImage(this.CursorImage,
                new Rect(
                    finalX,
                    finalY,
                    width,
                    height)
                );

            if (DrawOutline)
            {
                drawingContext.DrawRectangle(null, outlinePen, new Rect(finalX, finalY, width, height));
                drawingContext.DrawRoundedRectangle(null, shadowPen, new Rect(finalX - 1, finalY - 1, width + 2, height + 2), 2, 2);
            }
        }
    }
}
