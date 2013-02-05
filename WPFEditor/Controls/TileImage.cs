using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MegaMan.Editor.Controls
{
    public class TileImage : ContentControl
    {
        public static readonly DependencyProperty SheetPathProperty = DependencyProperty.Register("SheetPath", typeof(string), typeof(TileImage));

        private Image _image;

        public string SheetPath
        {
            get { return (string)GetValue(SheetPathProperty); }
            set { SetValue(SheetPathProperty, value); }
        }

        public TileImage()
        {
            ((App)App.Current).Tick += TileImage_Tick;

            this.DataContextChanged += TileImage_DataContextChanged;

            _image = new Image();
            Content = _image;
        }

        void TileImage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tile = (Tile)e.NewValue;

            _image.MinWidth = tile.Width;
            _image.MinHeight = tile.Height;
        }

        private void TileImage_Tick()
        {
            if (SheetPath != null)
            {
                var tile = (Tile)DataContext;

                var size = tile.Width;

                var location = tile.Sprite.CurrentFrame.SheetLocation;

                var image = SpriteBitmapCache.GetOrLoadFrame(SheetPath, location);

                _image.Source = image;
                _image.InvalidateVisual();
            }
        }
    }
}
