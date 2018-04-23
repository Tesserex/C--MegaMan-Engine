using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MegaMan.Common;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls {
    public class SpriteImage : Grid
    {
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double), typeof(SpriteImage), new PropertyMetadata(1d, new PropertyChangedCallback(ZoomChanged)));
        public static readonly DependencyProperty FlippedProperty = DependencyProperty.Register("Flipped", typeof(bool), typeof(SpriteImage), new PropertyMetadata(false));

        protected Image _image;
        private SpriteModel _sprite;
        
        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public bool Flipped
        {
            get { return (bool)GetValue(FlippedProperty); }
            set { SetValue(FlippedProperty, value); }
        }

        public SpriteImage()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ((App)App.Current).Tick += (s, e) => Tick();

                this.DataContextChanged += SpriteImage_DataContextChanged;
            }

            _image = new Image();
            Children.Add(_image);
            _image.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        protected virtual void SpriteImage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Sprite)
                SetSprite(new SpriteModel((Sprite)e.NewValue));

            if (e.NewValue is SpriteModel)
                SetSprite((SpriteModel)e.NewValue);

            if (e.NewValue == null)
            {
                _sprite = null;
                _image.Width = 0;
                _image.Height = 0;
                this.Width = 0;
                this.Height = 0;
            }
        }

        protected void SetSprite(SpriteModel s)
        {
            _sprite = s;
            _image.Width = s.Width * Zoom;
            _image.Height = s.Height * Zoom;
            this.Width = _image.Width;
            this.Height = _image.Height;
        }

        private static void ZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var image = (SpriteImage)d;
            image.Width = image._sprite.Width * (double)e.NewValue;
            image.Height = image._sprite.Height * (double)e.NewValue;
            image._image.Width = image.Width;
            image._image.Height = image.Height;
            image.Tick();
        }

        protected virtual void Tick()
        {
            if (_sprite == null)
                return;

            var image = _sprite.GetImageSource(Zoom);

            if (_sprite.Reversed ^ Flipped)
                _image.RenderTransform = new ScaleTransform(-1, 1);
            else
                _image.RenderTransform = null;

            _image.Source = image;
            _image.InvalidateVisual();
        }
    }
}
