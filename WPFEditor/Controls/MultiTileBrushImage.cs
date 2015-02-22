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
        public static readonly DependencyProperty HighlightProperty = DependencyProperty.Register("Highlight", typeof(bool), typeof(MultiTileBrushImage), new PropertyMetadata(new PropertyChangedCallback(HighlightChanged)));

        protected List<Image> _images;
        protected Border _highlight;
        private MultiTileBrush _brush;

        public bool Highlight
        {
            get { return (bool)GetValue(HighlightProperty); }
            set { SetValue(HighlightProperty, value); }
        }

        public MultiTileBrushImage()
        {
            ((App)App.Current).Tick += Tick;

            this.DataContextChanged += Image_DataContextChanged;

            _images = new List<Image>();

            _highlight = new Border() { BorderThickness = new Thickness(1.5), BorderBrush = Brushes.Yellow, Width = 16, Height = 16 };
            _highlight.Effect = new BlurEffect() { Radius = 2 };
            _highlight.Visibility = System.Windows.Visibility.Hidden;
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
                this.Children.Remove(i);
            }

            var width = _brush.Cells.Length;
            var height = _brush.Cells[0].Length;

            this.Width = _brush.Cells[0][0].tile.Width * width;
            this.Height = _brush.Cells[0][0].tile.Height * height;

            this.ColumnDefinitions.Clear();
            this.RowDefinitions.Clear();

            for (var x = 0; x < width; x++)
            {
                var col = new ColumnDefinition();
                col.Width = new GridLength(_brush.Cells[0][0].tile.Width);
                this.ColumnDefinitions.Add(col);

                for (var y = 0; y < height; y++)
                {
                    if (x == 0)
                    {
                        var row = new RowDefinition();
                        row.Height = new GridLength(_brush.Cells[0][0].tile.Height);
                        this.RowDefinitions.Add(row);
                    }

                    var cell = _brush.Cells[x][y];
                    var image = new Image();
                    image.Width = cell.tile.Width;
                    image.Height = cell.tile.Height;

                    Grid.SetColumn(image, x);
                    Grid.SetRow(image, y);

                    this._images.Add(image);
                }
            }

            foreach (var i in _images)
            {
                this.Children.Add(i);
            }
        }

        private static void HighlightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

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
                    var image = this._images[x * height + y];

                    var location = cell.tile.Sprite.CurrentFrame.SheetLocation;

                    var source = SpriteBitmapCache.GetOrLoadFrame(cell.tile.Sprite.SheetPath.Absolute, location);

                    image.Source = source;
                    image.InvalidateVisual();
                }
            }
        }
    }
}
