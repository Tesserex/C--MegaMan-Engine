using MegaMan.Common;
using MegaMan.Editor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MegaMan.Editor.Tools
{
    public class SingleTileCursor : IToolCursor
    {
        private Tileset _tileset;
        private Tile _tile;

        private ToolCursorAdorner _cursorAdorner;

        private FrameworkElement _element;

        public SingleTileCursor(Tileset tileset, Tile tile)
        {
            _tileset = tileset;
            _tile = tile;
        }

        private ImageSource CursorImage
        {
            get { return SpriteBitmapCache.GetOrLoadFrame(_tileset.SheetPath.Absolute, _tile.Sprite.CurrentFrame.SheetLocation); }
        }

        public void ApplyCursorTo(FrameworkElement element)
        {
            _element = element;

            _cursorAdorner = new ToolCursorAdorner(element, Render);
            AdornerLayer.GetAdornerLayer(element).Add(_cursorAdorner);

            WeakEventManager<FrameworkElement, MouseEventArgs>.AddHandler(element, "MouseEnter", MouseEnter);
            WeakEventManager<FrameworkElement, MouseEventArgs>.AddHandler(element, "MouseLeave", MouseLeave);

            element.Cursor = Cursors.None;
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
            var cursorPosition = Mouse.GetPosition(_element);

            drawingContext.DrawImage(this.CursorImage,
                new Rect(
                    cursorPosition.X - (_tile.Width / 2),
                    cursorPosition.Y - (_tile.Height / 2),
                    _tile.Width,
                    _tile.Height)
                );
        }
    }
}
