using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MegaMan.Common;

namespace MegaMan.Editor.Controls
{
    public class TileImage : SpriteImage
    {
        public static readonly DependencyProperty SelectedTileProperty = DependencyProperty.Register("SelectedTile", typeof(Tile), typeof(TileImage), new PropertyMetadata(new PropertyChangedCallback(SelectedTileChanged)));

        public Tile SelectedTile
        {
            get { return (Tile)GetValue(SelectedTileProperty); }
            set { SetValue(SelectedTileProperty, value); }
        }

        protected Border _highlight;

        public TileImage() : base()
        {
            _highlight = new Border() { BorderThickness = new Thickness(1.5), BorderBrush = Brushes.Yellow, Width = 16, Height = 16 };
            _highlight.Effect = new BlurEffect() { Radius = 2 };
            _highlight.Visibility = System.Windows.Visibility.Hidden;
            Children.Add(_highlight);
        }

        protected override void SpriteImage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tile = (Tile)e.NewValue;

            SetSprite(tile.Sprite);
        }

        private static void SelectedTileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var image = (TileImage)sender;

            image._highlight.Visibility = (image.SelectedTile == image.DataContext) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
