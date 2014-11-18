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

        private Image _image;
        private Border _highlight;

        public bool Highlight
        {
            get { return (bool)GetValue(HighlightProperty); }
            set { SetValue(HighlightProperty, value); }
        }

        public SpriteImage()
        {
            ((App)App.Current).Tick += Tick;

            this.DataContextChanged += SpriteImage_DataContextChanged;

            _image = new Image();
            Children.Add(_image);

            _highlight = new Border() { BorderThickness = new Thickness(1.5), BorderBrush = Brushes.Yellow, Width = 16, Height = 16 };
            _highlight.Effect = new BlurEffect() { Radius = 2 };
            _highlight.Visibility = System.Windows.Visibility.Hidden;
            Children.Add(_highlight);
        }

        void SpriteImage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is Sprite))
                return;

            var sprite = (Sprite)e.NewValue;

            _image.Width = sprite.Width;
            _image.Height = sprite.Height;
        }

        private static void HighlightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var image = (SpriteImage)sender;

            image._highlight.Visibility = image.Highlight ? Visibility.Visible : Visibility.Hidden;
        }

        private void Tick()
        {
            var sprite = (Sprite)DataContext;

            var size = sprite.Width;

            var location = sprite.CurrentFrame.SheetLocation;

            var image = SpriteBitmapCache.GetOrLoadFrame(sprite.SheetPath.Absolute, location);

            _image.Source = image;
            _image.InvalidateVisual();
        }
    }
}
