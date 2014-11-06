using System.Windows.Media;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls
{
    public class OverlayScreenLayer : ScreenLayer
    {
        private Rectangle? _selectionBounds;

        private static Pen _selectionPen = new Pen() {
            Brush = Brushes.Silver,
            DashStyle = DashStyles.Dash,
            DashCap = PenLineCap.Round,
            Thickness = 2
        };

        protected override void UnbindScreen(ScreenDocument oldScreen)
        {
            _selectionBounds = null;
            oldScreen.SelectionChanged -= UpdateSelection;
        }

        protected override void BindScreen(ScreenDocument newScreen)
        {
            newScreen.SelectionChanged += UpdateSelection;
            UpdateSelection(newScreen.Selection);
        }

        private void UpdateSelection(Rectangle? bounds)
        {
            _selectionBounds = bounds;
        }

        protected override void Update()
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_selectionBounds.HasValue)
            {
                var x = _selectionBounds.Value.X * Screen.Tileset.TileSize;
                var y = _selectionBounds.Value.Y * Screen.Tileset.TileSize;
                var w = _selectionBounds.Value.Width * Screen.Tileset.TileSize;
                var h = _selectionBounds.Value.Height * Screen.Tileset.TileSize;
                dc.DrawRectangle(null, _selectionPen, new System.Windows.Rect(x, y, w, h));
            }
        }
    }
}
