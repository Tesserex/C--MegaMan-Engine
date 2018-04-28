using System.Windows;
using MegaMan.Common;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls.Parts
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

            SetSprite(new SpriteModel(tile.Sprite));
        }
    }
}
