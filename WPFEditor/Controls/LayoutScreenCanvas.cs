using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MegaMan.Editor.Controls
{
    public class LayoutScreenCanvas : ScreenCanvas
    {
        private bool _dragging;
        private Vector _dragAnchorOffset;

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            _highlight.Visibility = System.Windows.Visibility.Visible;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            _highlight.Visibility = System.Windows.Visibility.Hidden;
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            BeginLayoutDrag(e.GetPosition(this));
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            EndLayoutDrag();
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_dragging)
            {
                var mousePosition = e.GetPosition((IInputElement)this.Parent);  

                this.Margin = new Thickness(mousePosition.X - _dragAnchorOffset.X, mousePosition.Y - _dragAnchorOffset.Y, 0, 0);
            }
        }

        private void BeginLayoutDrag(Point anchor)
        {
            _dragging = true;
            _dragAnchorOffset = new Vector(anchor.X, anchor.Y);

            CaptureMouse();

            Canvas.SetZIndex(this, 100);
        }

        private void EndLayoutDrag()
        {
            _dragging = false;

            ReleaseMouseCapture();

            Canvas.SetZIndex(this, 1);
        }
    }
}
