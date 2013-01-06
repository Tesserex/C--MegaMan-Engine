using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MegaMan.Editor.Controls
{
    public class GridCanvas : Canvas
    {
        private static Pen _gridPen;

        static GridCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridCanvas), new FrameworkPropertyMetadata(typeof(GridCanvas)));

            _gridPen = new Pen(Brushes.DarkGray, 1);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(Brushes.WhiteSmoke, null, new Rect(this.RenderSize));

            // grid lines
            for (int i = 16; i < this.ActualWidth; i += 16)
            {
                drawingContext.DrawLine(_gridPen, new Point(i, 0), new Point(i, this.ActualHeight));
            }

            for (int i = 16; i < this.ActualHeight; i += 16)
            {
                drawingContext.DrawLine(_gridPen, new Point(0, i), new Point(this.ActualWidth, i));
            }
        }
    }
}
