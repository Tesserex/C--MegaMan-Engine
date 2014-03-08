using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace MegaMan.Editor.Controls
{
    public class TileImage : Grid
    {
        public static readonly DependencyProperty SelectedTileProperty = DependencyProperty.Register("SelectedTile", typeof(Tile), typeof(TileImage), new PropertyMetadata(new PropertyChangedCallback(SelectedTileChanged)));

        private Image _image;
        private Border _highlight;

        public Tile SelectedTile
        {
            get { return (Tile)GetValue(SelectedTileProperty); }
            set { SetValue(SelectedTileProperty, value); }
        }

        public TileImage()
        {
            ((App)App.Current).Tick += TileImage_Tick;

            this.DataContextChanged += TileImage_DataContextChanged;

            _image = new Image();
            Children.Add(_image);

            _highlight = new Border() { BorderThickness = new Thickness(1.5), BorderBrush = Brushes.Yellow, Width = 16, Height = 16 };
            _highlight.Effect = new BlurEffect() { Radius = 2 };
            _highlight.Visibility = System.Windows.Visibility.Hidden;
            Children.Add(_highlight);
        }

        void TileImage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tile = (Tile)e.NewValue;

            _image.MinWidth = tile.Width;
            _image.MinHeight = tile.Height;
        }

        private static void SelectedTileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var image = (TileImage)sender;

            image._highlight.Visibility = (image.SelectedTile == image.DataContext) ? Visibility.Visible : Visibility.Hidden;
        }

        private void TileImage_Tick()
        {
            var tile = (Tile)DataContext;

            var size = tile.Width;

            var location = tile.Sprite.CurrentFrame.SheetLocation;

            var image = SpriteBitmapCache.GetOrLoadFrame(tile.Sprite.SheetPath.Absolute, location);

            _image.Source = image;
            _image.InvalidateVisual();
        }
    }
}
