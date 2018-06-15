using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MegaMan.Editor.Controls
{
    public class GridCanvas : Grid
    {
        private static Pen _gridPen;

        static GridCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridCanvas), new FrameworkPropertyMetadata(typeof(GridCanvas)));

            _gridPen = new Pen(Brushes.DarkGray, 1);
            if (_gridPen.CanFreeze)
            {
                _gridPen.Freeze();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(Brushes.WhiteSmoke, null, new Rect(RenderSize));

            // grid lines
            for (int i = 16; i < ActualWidth; i += 16)
            {
                drawingContext.DrawLine(_gridPen, new Point(i, 0), new Point(i, ActualHeight));
            }

            for (int i = 16; i < ActualHeight; i += 16)
            {
                drawingContext.DrawLine(_gridPen, new Point(0, i), new Point(ActualWidth, i));
            }
        }
    }
}
