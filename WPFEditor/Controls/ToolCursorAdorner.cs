using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace MegaMan.Editor.Controls
{
    public class ToolCursorAdorner : Adorner
    {
        private Action<DrawingContext> _renderAction;

        public ToolCursorAdorner(UIElement adornedElement, Action<DrawingContext> renderAction) : base(adornedElement)
        {
            _renderAction = renderAction;

            IsHitTestVisible = false;
            SnapsToDevicePixels = true;
            UseLayoutRounding = true;

            ((App)App.Current).Tick += Tick;
            adornedElement.MouseMove += Tick;
        }

        private void Tick(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            _renderAction(drawingContext);
        }
    }
}
