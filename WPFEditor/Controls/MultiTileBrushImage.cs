using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MegaMan.Editor.Bll.Tools;

namespace MegaMan.Editor.Controls
{
    public class MultiTileBrushImage : Grid
    {
        public static readonly DependencyProperty HighlightProperty = DependencyProperty.Register("Highlight", typeof(bool), typeof(MultiTileBrushImage), new PropertyMetadata(HighlightChanged));
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(int), typeof(MultiTileBrushImage), new PropertyMetadata(1, ZoomChanged));

        protected List<Image> _images;
        protected Border _highlight;
        private MultiTileBrush _brush;

        public bool Highlight
        {
            get { return (bool)GetValue(HighlightProperty); }
            set { SetValue(HighlightProperty, value); }
        }

        public int Zoom
        {
            get { return (int)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public MultiTileBrushImage()
        {
            ((App)App.Current).Tick += (s, e) => Tick();
            
            DataContextChanged += Image_DataContextChanged;

            _images = new List<Image>();

            _highlight = new Border { BorderThickness = new Thickness(1.5), BorderBrush = Brushes.Yellow, Width = 16, Height = 16 };
            _highlight.Effect = new BlurEffect { Radius = 2 };
            _highlight.Visibility = Visibility.Hidden;
            Children.Add(_highlight);
        }

        protected virtual void Image_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is MultiTileBrush))
                return;

            SetBrush((MultiTileBrush)e.NewValue);
        }

        protected void SetBrush(MultiTileBrush s)
        {
            _brush = s;

            foreach (var i in _images)
            {
                Children.Remove(i);
            }

            var width = _brush.Cells.Length;
            var height = _brush.Cells[0].Length;

            var cellWidth = _brush.Cells[0][0].tile.Width * Zoom;
            var cellHeight = _brush.Cells[0][0].tile.Height * Zoom;

            Width = cellWidth * width;
            Height = cellHeight * height;

            ColumnDefinitions.Clear();
            RowDefinitions.Clear();

            for (var x = 0; x < width; x++)
            {
                var col = new ColumnDefinition();
                col.Width = new GridLength(cellWidth);
                ColumnDefinitions.Add(col);

                for (var y = 0; y < height; y++)
                {
                    if (x == 0)
                    {
                        var row = new RowDefinition();
                        row.Height = new GridLength(cellHeight);
                        RowDefinitions.Add(row);
                    }

                    var cell = _brush.Cells[x][y];
                    var image = new Image();
                    image.Width = cellWidth;
                    image.Height = cellHeight;

                    SetColumn(image, x);
                    SetRow(image, y);

                    _images.Add(image);
                }
            }

            foreach (var i in _images)
            {
                Children.Add(i);
            }
        }

        private static void HighlightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }

        private static void ZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (MultiTileBrushImage)d;

            var zoom = (int)e.NewValue;
            var cells = ctrl._brush.Cells;
            var tilesize = cells[0][0].tile.Width;
            var width = cells.Length;
            var height = cells[0].Length;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var image = ctrl._images[x * height + y];
                    image.Width = tilesize * zoom;
                    image.Height = tilesize * zoom;
                }

                ctrl.ColumnDefinitions[x].Width = new GridLength(tilesize * zoom);
            }

            for (var y = 0; y < height; y++)
            {
                ctrl.RowDefinitions[y].Height = new GridLength(tilesize * zoom);
            }

            ctrl.Width = tilesize * width * zoom;
            ctrl.Height = tilesize * height * zoom;
            ctrl.Tick();
        }

        protected virtual void Tick()
        {
            if (_brush == null)
                return;

            var width = _brush.Cells.Length;
            var height = _brush.Cells[0].Length;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var cell = _brush.Cells[x][y];
                    var image = _images[x * height + y];

                    var location = cell.tile.Sprite[0].SheetLocation;

                    var source = SpriteBitmapCache.GetOrLoadFrame(cell.tile.Sprite.SheetPath.Absolute, location);
                    source = SpriteBitmapCache.Scale(source, Zoom);

                    image.Source = source;
                    image.InvalidateVisual();
                }
            }
        }
    }
}
