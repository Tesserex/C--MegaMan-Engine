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

        public TileImage() : base()
        {
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
