using System.Windows;
using MegaMan.Common;

namespace MegaMan.Editor.Controls
{
    public class TileImage : SpriteImage
    {
        public TileImage()
            : base()
        {
        }

        protected override void SpriteImage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tile = (Tile)e.NewValue;

            SetSprite(tile.Sprite);
        }
    }
}
