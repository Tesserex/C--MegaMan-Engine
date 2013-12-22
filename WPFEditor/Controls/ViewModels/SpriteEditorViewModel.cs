using MegaMan.Common;
using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class SpriteEditorViewModel : INotifyPropertyChanged
    {
        private Sprite _sprite;

        private int _zoomFactor = 1;

        private const int MAXZOOM = 16;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }

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

            ZoomInCommand = new RelayCommand(ZoomIn, CanZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut, CanZoomOut);
        }

        public void ChangeSprite(Sprite sprite)
        {
            _sprite = sprite;

            OnPropertyChanged("PreviewWidth");
            OnPropertyChanged("PreviewHeight");
            OnPropertyChanged("SheetImageSource");
            OnPropertyChanged("PreviewImage");
        }

        private bool CanZoomOut(object obj)
        {
            return _zoomFactor > 1;
        }

        private bool CanZoomIn(object obj)
        {
            return _zoomFactor < MAXZOOM;
        }

        private void ZoomOut(object obj)
        {
            _zoomFactor = Math.Max(1, _zoomFactor / 2);
            OnPropertyChanged("PreviewWidth");
            OnPropertyChanged("PreviewHeight");
        }

        private void ZoomIn(object obj)
        {
            _zoomFactor = Math.Min(MAXZOOM, _zoomFactor * 2);
            OnPropertyChanged("PreviewWidth");
            OnPropertyChanged("PreviewHeight");
        }

        public BitmapSource SheetImageSource
        {
            get
            {
                return SpriteBitmapCache.GetOrLoadImage(_sprite.SheetPath.Absolute);
            }
        }

        public int PreviewWidth
        {
            get
            {
                return _sprite.Width * _zoomFactor;
            }
        }

        public int PreviewHeight
        {
            get
            {
                return _sprite.Height * _zoomFactor;
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
