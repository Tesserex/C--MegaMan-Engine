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
        public static readonly DependencyProperty HighlightProperty = DependencyProperty.Register("Highlight", typeof(bool), typeof(SpriteImage), new PropertyMetadata(new PropertyChangedCallback(HighlightChanged)));

        protected Image _image;
        protected Border _highlight;
        private Sprite _sprite;

        public bool Highlight
        {
            get { return (bool)GetValue(HighlightProperty); }
            set { SetValue(HighlightProperty, value); }
        }

        public SpriteImage()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ((App)App.Current).Tick += Tick;

                this.DataContextChanged += SpriteImage_DataContextChanged;
            }

            _image = new Image();
            Children.Add(_image);

            _highlight = new Border() { BorderThickness = new Thickness(1.5), BorderBrush = Brushes.Yellow, Width = 16, Height = 16 };
            _highlight.Effect = new BlurEffect() { Radius = 2 };
            _highlight.Visibility = System.Windows.Visibility.Hidden;
            Children.Add(_highlight);
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

        private static void HighlightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var image = (SpriteImage)sender;

            image._highlight.Visibility = image.Highlight ? Visibility.Visible : Visibility.Hidden;
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
