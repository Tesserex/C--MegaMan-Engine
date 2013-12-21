using MegaMan.Common;
using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class SpriteEditorViewModel : INotifyPropertyChanged
    {
        private Sprite _sprite;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public SpriteEditorViewModel(Sprite sprite)
        {
            _sprite = sprite;

            ((App)App.Current).Tick -= Update;
            ((App)App.Current).Tick += Update;
        }

        public ImageSource SheetImageSource
        {
            get
            {
                var rect = new Rectangle(0, 0, _sprite.Width, _sprite.Height);
                return SpriteBitmapCache.GetOrLoadFrame(_sprite.SheetPath.Absolute, rect);
            }
        }

        public int PreviewWidth
        {
            get
            {
                return _sprite.Width;
            }
        }

        public int PreviewHeight
        {
            get
            {
                return _sprite.Height;
            }
        }

        public ImageSource PreviewImage
        {
            get
            {
                var rect = _sprite.CurrentFrame.SheetLocation;
                return SpriteBitmapCache.GetOrLoadFrame(_sprite.SheetPath.Absolute, rect);
            }
        }

        public void PlayPreview()
        {
            _sprite.Play();
        }

        public void PausePreview()
        {
            _sprite.Pause();
        }

        private void Update()
        {
            _sprite.Update();
            OnPropertyChanged("PreviewImage");
        }
    }
}
