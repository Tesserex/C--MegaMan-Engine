using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class SpriteEditorViewModel : INotifyPropertyChanged
    {
        private ProjectDocument _project;

        private static int _previewZoom = 1;
        private static int _sheetZoom = 1;

        public int SheetZoom { get { return _sheetZoom; } }

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public SpriteEditorViewModel(SpriteViewModel sprite, ProjectDocument project = null)
        {
            Sprite = sprite ?? throw new ArgumentNullException("sprite");
            _project = project;

            ((App)App.Current).Tick += (s, e) => Update();

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
            if (Sprite.CurrentIndex == Sprite.Count - 1)
                Sprite.CurrentIndex = 0;
            else
                Sprite.CurrentIndex++;

            Update();
        }

        private void PreviousFrame()
        {
            if (Sprite.CurrentIndex == 0)
                Sprite.CurrentIndex = Sprite.Count - 1;
            else
                Sprite.CurrentIndex--;

            Update();
        }

        private void AddFrame()
        {
            Sprite.InsertFrame(Sprite.CurrentIndex + 1);
            Sprite.CurrentIndex = Sprite.Count - 1;

            if (_project != null)
                _project.Dirty = true;

            Update();
        }

        private void DeleteFrame()
        {
            Sprite.Remove(Sprite.CurrentFrame);

            if (_project != null)
                _project.Dirty = true;
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
            RefreshSheet();
        }

        private void ZoomInSheet(object obj)
        {
            _sheetZoom = Math.Min(MAXZOOM, _sheetZoom * 2);
            RefreshSheet();
        }

        public void RefreshSheet()
        {
            OnPropertyChanged("SheetWidth");
            OnPropertyChanged("SheetHeight");
            OnPropertyChanged("HighlightWidth");
            OnPropertyChanged("HighlightHeight");
            OnPropertyChanged("SheetImageSource");
        }

        public SpriteViewModel Sprite { get; private set; }

        public string Name
        {
            get { return Sprite.Name; }
            set
            {
                Sprite.Name = value;
                if (_project != null)
                    _project.Dirty = true;
            }
        }

        public int FrameNumber
        {
            get
            {
                return Sprite.CurrentIndex;
            }
        }

        public int FrameDuration
        {
            get
            {
                return Sprite.CurrentFrame.Duration;
            }
            set
            {
                Sprite.CurrentFrame.Duration = value;

                if (_project != null)
                    _project.Dirty = true;
            }
        }

        public int SpriteWidth
        {
            get
            {
                return Sprite.Width;
            }
            set
            {
                Sprite.Width = value;

                if (_project != null)
                    _project.Dirty = true;

                OnPropertyChanged("PreviewWidth");
                OnPropertyChanged("PreviewHeight");
                OnPropertyChanged("HighlightWidth");
                OnPropertyChanged("HighlightHeight");
                OnPropertyChanged("PreviewImage");
            }
        }

        public int SpriteHeight
        {
            get
            {
                return Sprite.Height;
            }
            set
            {
                Sprite.Height = value;

                if (_project != null)
                    _project.Dirty = true;

                OnPropertyChanged("PreviewWidth");
                OnPropertyChanged("PreviewHeight");
                OnPropertyChanged("HighlightWidth");
                OnPropertyChanged("HighlightHeight");
                OnPropertyChanged("PreviewImage");
            }
        }

        public bool Reversed
        {
            get
            {
                return Sprite.Reversed;
            }
            set
            {
                Sprite.Reversed = value;

                if (_project != null)
                    _project.Dirty = true;
            }
        }

        public BitmapSource SheetImageSource
        {
            get
            {
                if (Sprite.SheetPath == null)
                    return null;

                if (Sprite.Playing)
                    return SpriteBitmapCache.GetOrLoadImageGrayscale(Sprite.SheetPath.Absolute);
                return SpriteBitmapCache.GetOrLoadImage(Sprite.SheetPath.Absolute);
            }
        }

        public int PreviewWidth
        {
            get
            {
                return Sprite.Width * _previewZoom;
            }
        }

        public int PreviewHeight
        {
            get
            {
                return Sprite.Height * _previewZoom;
            }
        }

        public ImageSource PreviewImage
        {
            get
            {
                if (Sprite.SheetPath == null)
                    return null;

                var rect = Sprite.CurrentFrame.SheetLocation;
                return SpriteBitmapCache.GetOrLoadFrame(Sprite.SheetPath.Absolute, rect);
            }
        }

        private Color transparentColor;
        public Color TransparentColor
        {
            get { return transparentColor; }
            set
            {
                transparentColor = value;
                OnPropertyChanged(nameof(TransparentBrush));
            }
        }

        public Brush TransparentBrush
        {
            get { return new SolidColorBrush(TransparentColor); }
        }

        public Cursor SheetCursor
        {
            get
            {
                if (Sprite.Playing)
                    return Cursors.Arrow;
                return Cursors.None;
            }
        }

        public double SheetWidth
        {
            get
            {
                var source = SheetImageSource;
                if (source == null)
                    return 0;

                return source.PixelWidth * _sheetZoom;
            }
        }

        public double SheetHeight
        {
            get
            {
                var source = SheetImageSource;
                if (source == null)
                    return 0;

                return source.PixelHeight * _sheetZoom;
            }
        }

        public int HighlightWidth
        {
            get
            {
                return Sprite.Width * _sheetZoom;
            }
        }

        public int HighlightHeight
        {
            get
            {
                return Sprite.Height * _sheetZoom;
            }
        }

        public void PlayPreview()
        {
            Sprite.Play();
            OnPropertyChanged("SheetImageSource");
            OnPropertyChanged("SheetCursor");
        }

        public void PausePreview()
        {
            Sprite.Pause();
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

            Sprite.CurrentFrame.SetSheetPosition(x, y);

            if (_project != null)
                _project.Dirty = true;

            OnPropertyChanged("PreviewImage");
        }
    }
}
