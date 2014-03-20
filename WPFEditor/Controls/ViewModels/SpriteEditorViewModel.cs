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

        private int _previewZoom = 1;
        private int _sheetZoom = 1;

        private const int MAXZOOM = 16;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand ZoomInSheetCommand { get; private set; }
        public ICommand ZoomOutSheetCommand { get; private set; }
        public ICommand PlayCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand PreviousFrameCommand { get; private set; }
        public ICommand NextFrameCommand { get; private set; }
        public ICommand AddFrameCommand { get; private set; }
        public ICommand DeleteFrameCommand { get; private set; }

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
            if (sprite == null)
                throw new ArgumentNullException("sprite");

            _sprite = sprite;

            ((App)App.Current).Tick += Update;

            ZoomInCommand = new RelayCommand(ZoomIn, CanZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut, CanZoomOut);
            ZoomInSheetCommand = new RelayCommand(ZoomInSheet, CanZoomInSheet);
            ZoomOutSheetCommand = new RelayCommand(ZoomOutSheet, CanZoomOutSheet);
            PlayCommand = new RelayCommand(p => PlayPreview(), p => !Sprite.Playing);
            PauseCommand = new RelayCommand(p => PausePreview(), p => Sprite.Playing);
            PreviousFrameCommand = new RelayCommand(p => PreviousFrame(), p => !Sprite.Playing);
            NextFrameCommand = new RelayCommand(p => NextFrame(), p => !Sprite.Playing);
            AddFrameCommand = new RelayCommand(p => AddFrame(), p => !Sprite.Playing);
            DeleteFrameCommand = new RelayCommand(p => DeleteFrame(), p => !Sprite.Playing && Sprite.Count > 1);
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

        private void AddFrame()
        {
            _sprite.InsertFrame(_sprite.CurrentIndex + 1);

            Update();
        }

        private void DeleteFrame()
        {
            _sprite.Remove(_sprite.CurrentFrame);
        }

        private bool CanZoomOut(object obj)
        {
            return _previewZoom > 1;
        }

        private bool CanZoomIn(object obj)
        {
            return _previewZoom < MAXZOOM;
        }

        private void ZoomOut(object obj)
        {
            _previewZoom = Math.Max(1, _previewZoom / 2);
            OnPropertyChanged("PreviewWidth");
            OnPropertyChanged("PreviewHeight");
        }

        private void ZoomIn(object obj)
        {
            _previewZoom = Math.Min(MAXZOOM, _previewZoom * 2);
            OnPropertyChanged("PreviewWidth");
            OnPropertyChanged("PreviewHeight");
        }

        private bool CanZoomOutSheet(object obj)
        {
            return _sheetZoom > 1;
        }

        private bool CanZoomInSheet(object obj)
        {
            return _sheetZoom < MAXZOOM;
        }

        private void ZoomOutSheet(object obj)
        {
            _sheetZoom = Math.Max(1, _sheetZoom / 2);
            OnPropertyChanged("SheetWidth");
            OnPropertyChanged("SheetHeight");
            OnPropertyChanged("HighlightWidth");
            OnPropertyChanged("HighlightHeight");
        }

        private void ZoomInSheet(object obj)
        {
            _sheetZoom = Math.Min(MAXZOOM, _sheetZoom * 2);
            OnPropertyChanged("SheetWidth");
            OnPropertyChanged("SheetHeight");
            OnPropertyChanged("HighlightWidth");
            OnPropertyChanged("HighlightHeight");
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

        public int FrameDuration
        {
            get
            {
                return _sprite.CurrentFrame.Duration;
            }
            set
            {
                _sprite.CurrentFrame.Duration = value;
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
                return _sprite.Width * _previewZoom;
            }
        }

        public int PreviewHeight
        {
            get
            {
                return _sprite.Height * _previewZoom;
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

        public double SheetWidth
        {
            get
            {
                return SheetImageSource.PixelWidth * _sheetZoom;
            }
        }

        public double SheetHeight
        {
            get
            {
                return SheetImageSource.PixelHeight * _sheetZoom;
            }
        }

        public int HighlightWidth
        {
            get
            {
                return _sprite.Width * _sheetZoom;
            }
        }

        public int HighlightHeight
        {
            get
            {
                return _sprite.Height * _sheetZoom;
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
            OnPropertyChanged("FrameDuration");
        }

        public void SetFrameLocation(int x, int y)
        {
            x /= _sheetZoom;
            y /= _sheetZoom;

            _sprite.CurrentFrame.SetSheetPosition(x, y);
            OnPropertyChanged("PreviewImage");
        }
    }
}
