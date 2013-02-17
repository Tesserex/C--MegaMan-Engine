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
        public IToolProvider ToolProvider { get; set; }

        public ToolCursorAdorner(UIElement adornedElement) : base(adornedElement)
        {
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

            if (ToolProvider != null && ToolProvider.ToolCursor != null)
            {
                var cursorPosition = Mouse.GetPosition(this);

                drawingContext.DrawImage(ToolProvider.ToolCursor, new Rect(cursorPosition.X - 8, cursorPosition.Y - 8, 16, 16));
            }
        }
    }
}
