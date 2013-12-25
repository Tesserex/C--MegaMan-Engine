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
        public ICommand PlayCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand PreviousFrameCommand { get; private set; }
        public ICommand NextFrameCommand { get; private set; }

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

            ((App)App.Current).AnimateSprite(sprite);
            ((App)App.Current).Tick += Update;

            ZoomInCommand = new RelayCommand(ZoomIn, CanZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut, CanZoomOut);
            PlayCommand = new RelayCommand(p => PlayPreview(), p => !Sprite.Playing);
            PauseCommand = new RelayCommand(p => PausePreview(), p => Sprite.Playing);
            PreviousFrameCommand = new RelayCommand(p => PreviousFrame(), p => !Sprite.Playing);
            NextFrameCommand = new RelayCommand(p => NextFrame(), p => !Sprite.Playing);
        }

        private void NextFrame()
        {
            if (_sprite.CurrentIndex == _sprite.Count - 1)
                _sprite.CurrentIndex = 0;
            else
                _sprite.CurrentIndex++;

            Update();
        }

        private void PreviousFrame()
        {
            if (_sprite.CurrentIndex == 0)
                _sprite.CurrentIndex = _sprite.Count - 1;
            else
                _sprite.CurrentIndex--;

            Update();
        }

        public void ChangeSprite(Sprite sprite)
        {
            _sprite = sprite;

            OnPropertyChanged("PreviewWidth");
            OnPropertyChanged("PreviewHeight");
            OnPropertyChanged("SheetImageSource");
            OnPropertyChanged("PreviewImage");
            OnPropertyChanged("Sprite");
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

        public Sprite Sprite
        {
            get
            {
                return _sprite;
            }
        }

        public int FrameNumber
        {
            get
            {
                return _sprite.CurrentIndex;
            }
        }

        public BitmapSource SheetImageSource
        {
            get
            {
                if (_sprite.Playing)
                    return SpriteBitmapCache.GetOrLoadImageGrayscale(_sprite.SheetPath.Absolute);
                else
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

        public Cursor SheetCursor
        {
            get
            {
                if (_sprite.Playing)
                    return Cursors.Arrow;
                else
                    return Cursors.None;
            }
        }

        public void PlayPreview()
        {
            _sprite.Play();
            OnPropertyChanged("SheetImageSource");
            OnPropertyChanged("SheetCursor");
        }

        public void PausePreview()
        {
            _sprite.Pause();
            OnPropertyChanged("SheetImageSource");
            OnPropertyChanged("SheetCursor");
        }

        private void Update()
        {
            OnPropertyChanged("PreviewImage");
            OnPropertyChanged("FrameNumber");
        }

        public void SetFrameLocation(int x, int y)
        {
            _sprite.CurrentFrame.SetSheetPosition(x, y);
            OnPropertyChanged("PreviewImage");
        }
    }
}
