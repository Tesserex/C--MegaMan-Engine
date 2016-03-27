using System.IO;
using System.Windows.Media.Imaging;

namespace MegaMan.Editor.Controls.ViewModels.Dialogs
{
    public class TilesetImageImportDialogViewModel : ViewModelBase
    {
        public BitmapSource Image { get; private set; }

        public string Title { get; private set; }

        public int ImageWidth { get; private set; }
        public int ImageHeight { get; private set; }

        private int _spacing;
        public int Spacing
        {
            get { return _spacing; }
            set
            {
                _spacing = value;
                OnPropertyChanged();
            }
        }

        private int _offset;
        public int Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                OnPropertyChanged();
            }
        }

        public TilesetImageImportDialogViewModel(string imagePath, int snap = 0, int offset = 0)
        {
            Image = SpriteBitmapCache.GetOrLoadImage(imagePath);
            ImageWidth = Image.PixelWidth;
            ImageHeight = Image.PixelHeight;

            var filename = Path.GetFileNameWithoutExtension(imagePath);
            Title = string.Format("Import {0}", filename);

            OnPropertyChanged("Image");
            OnPropertyChanged("ImageWidth");
            OnPropertyChanged("ImageHeight");
            OnPropertyChanged("Title");

            Spacing = snap;
            Offset = offset;
        }
    }
}
