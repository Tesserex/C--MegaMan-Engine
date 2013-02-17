using MegaMan.Editor.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MegaMan.Editor.Controls
{
    public class ToolCursorAdorner : Adorner
    {
        private IToolProvider _toolProvider;

        public ToolCursorAdorner(UIElement adornedElement, IToolProvider toolProvider) : base(adornedElement)
        {
            _toolProvider = toolProvider;

            IsHitTestVisible = false;
            SnapsToDevicePixels = true;
            UseLayoutRounding = true;

            ((App)App.Current).Tick += Tick;
        }

        private void Tick()
        {
            InvalidateVisual();
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_toolProvider != null && _toolProvider.ToolCursor != null)
            {
                var cursor = _toolProvider.ToolCursor;

                var cursorPosition = Mouse.GetPosition(this);

                drawingContext.DrawImage(cursor.CursorImage,
                    new Rect(
                        cursorPosition.X - (cursor.CursorWidth / 2),
                        cursorPosition.Y - (cursor.CursorHeight / 2),
                        cursor.CursorWidth,
                        cursor.CursorHeight)
                    );
            }
        }
    }
}
