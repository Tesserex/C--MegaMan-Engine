using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Algorithms;
using MegaMan.Editor.Bll.Tools;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Tools;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class LayoutEditingViewModel : IToolProvider, INotifyPropertyChanged
    {
        private IToolCursor _toolCursor;

        private IToolBehavior _toolBehavior;

        private StageDocument _currentStage;

        private IEntityImage _playerSprite;

        private string _activeIcon;

        public event EventHandler<ToolChangedEventArgs> ToolChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public LayoutEditingViewModel()
        {
            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);
            ViewModelMediator.Current.GetEvent<TestLocationClickedEventArgs>().Subscribe((s,e) => TestFromLocation());

            AddScreenCommand = new RelayCommand(p => AddScreen(), p => HasStage());
            ImportScreenCommand = new RelayCommand(p => ImportScreen(), p => HasStage());
            ChangeToolCommand = new RelayCommand(ChangeTool, p => HasStage());
        }

        public ICommand AddScreenCommand { get; private set; }
        public ICommand ImportScreenCommand { get; private set; }
        public ICommand ChangeToolCommand { get; private set; }

        private bool HasStage()
        {
            return (_currentStage != null);
        }

        public IToolBehavior Tool
        {
            get { return _toolBehavior; }
        }

        public IToolCursor ToolCursor
        {
            get
            {
                return _toolCursor;
            }
            private set
            {
                if (_toolCursor != null)
                {
                    _toolCursor.Dispose();
                }

                _toolCursor = value;
            }
        }

        public string CursorIcon { get { return IconFor("cursor"); } }
        public string CleaveIcon { get { return IconFor("cleave"); } }
        public string StartIcon { get { return IconFor("start"); } }
        public string ContinueIcon { get { return IconFor("continue"); } }

        private string ActiveIcon
        {
            get { return _activeIcon; }
            set
            {
                _activeIcon = value;
                OnPropertyChanged("CursorIcon");
                OnPropertyChanged("CleaveIcon");
                OnPropertyChanged("StartIcon");
                OnPropertyChanged("ContinueIcon");
            }
        }

        private string IconFor(string icon)
        {
            return String.Format("/Resources/{0}_{1}.png", icon, (_activeIcon == icon) ? "on" : "off");
        }

        private void AddScreen()
        {
            var stage = _currentStage;

            if (stage != null)
            {
                if (!stage.Tileset.Tileset.Any())
                {
                    CustomMessageBox.ShowError("You need to create some tiles before you can start creating your stage.", "Tiles Needed");
                    return;
                }

                int nextScreenId = stage.FindNextScreenId();

                var doc = stage.AddScreen(nextScreenId.ToString(), 16, 14);
                stage.PushHistoryAction(new AddScreenAction(doc));
            }
        }

        private void ImportScreen()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Images", "png,gif,jpg,jpeg,bmp"));

            dialog.Title = "Select Screen Image";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            var image = new BitmapImage(new Uri(dialog.FileName));

            var tilesize = _currentStage.Tileset.Tileset.TileSize;
            if (image.PixelWidth % tilesize != 0 || image.PixelHeight % tilesize != 0)
            {
                CustomMessageBox.ShowError(string.Format("Screen image width and height must be multiples of {0}.", tilesize), "Import Error");
                return;
            }

            var importer = new ScreenImporter(_currentStage);
            var screen = importer.Import(image);
            var document = _currentStage.AddScreen(screen);
            _currentStage.PushHistoryAction(new AddScreenAction(document));
        }

        private void DeleteScreen(ScreenDocument screen)
        {

        }

        private void ChangeTool(object toolParam)
        {
            switch (toolParam.ToString())
            {
                case "Hand":
                    ToolCursor = new StandardToolCursor("hand.cur");
                    _toolBehavior = new LayoutToolBehavior();
                    ActiveIcon = "cursor";
                    break;

                case "VSplit":
                    ToolCursor = new StandardToolCursor("vsplit.cur");
                    _toolBehavior = new CleaveScreenVerticalToolBehavior();
                    ActiveIcon = "cleave";
                    break;

                case "Start":
                    ToolCursor = new SpriteCursor(_playerSprite, 8, 1);
                    _toolBehavior = new StartPointToolBehavior(8, 1);
                    ActiveIcon = "start";
                    break;

                case "Continue":
                    ToolCursor = new SpriteCursor(_playerSprite);
                    _toolBehavior = new ContinuePointToolBehavior();
                    ActiveIcon = "continue";
                    break;
            }

            if (ToolChanged != null)
            {
                ToolChanged(this, new ToolChangedEventArgs(_toolBehavior));
            }
        }

        private void TestFromLocation()
        {
            ToolCursor = new SpriteCursor(_playerSprite);
            _toolBehavior = new TestLocationToolBehavior();
            ActiveIcon = null;

            if (ToolChanged != null)
            {
                ToolChanged(this, new ToolChangedEventArgs(_toolBehavior));
            }
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            _currentStage = e.Stage;

            if (_currentStage != null)
            {
                var player = _currentStage.Project.EntityByName("Player");
                if (player != null)
                {
                    _playerSprite = SpriteModel.ForEntity(player, _currentStage.Project);
                }
            }

            OnPropertyChanged("HasStage");
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
