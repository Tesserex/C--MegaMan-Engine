using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MegaMan.Common;

namespace MegaMan.Editor.Controls
{
    public class SpriteImage : Grid
    {
        protected Image _image;
        private Sprite _sprite;

        public SpriteImage()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ((App)App.Current).Tick += Tick;

                this.DataContextChanged += SpriteImage_DataContextChanged;
            }

            _image = new Image();
            Children.Add(_image);
        }

        protected virtual void SpriteImage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is Sprite))
                return;

            SetSprite((Sprite)e.NewValue);
        }

        protected void SetSprite(Sprite s)
        {
            _sprite = s;
            _image.Width = _sprite.Width;
            _image.Height = _sprite.Height;
        }

        protected virtual void Tick()
        {
            if (_sprite == null)
                return;

            var location = _sprite.CurrentFrame.SheetLocation;

            var image = SpriteBitmapCache.GetOrLoadFrame(_sprite.SheetPath.Absolute, location);

            _image.Source = image;
            _image.InvalidateVisual();
        }
    }
}
