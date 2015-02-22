using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Tools;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Tools;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class TilesetViewModel : TilesetViewModelBase, IToolProvider, INotifyPropertyChanged
    {
        private IToolBehavior _currentTool;
        private IToolCursor _currentCursor;
        private string _activeIcon;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ToolChangedEventArgs> ToolChanged;

        public ICommand ChangeToolCommand { get; private set; }

        public IToolBehavior Tool
        {
            get
            {
                return _currentTool;
            }
            private set
            {
                _currentTool = value;
            }
        }

        public IToolCursor ToolCursor
        {
            get
            {
                return _currentCursor;
            }
            private set
            {
                if (_currentCursor != null)
                {
                    _currentCursor.Dispose();
                }

                _currentCursor = value;
            }
        }

        public string BrushIcon { get { return IconFor("Brush"); } }
        public string BucketIcon { get { return IconFor("Bucket"); } }
        public string SelectionIcon { get { return IconFor("Selection"); } }
        public string RectangleIcon { get { return IconFor("Rectangle"); } }
        private string ActiveIcon
        {
            get { return _activeIcon; }
            set
            {
                _activeIcon = value;
                OnPropertyChanged("BrushIcon");
                OnPropertyChanged("BucketIcon");
                OnPropertyChanged("SelectionIcon");
                OnPropertyChanged("RectangleIcon");
            }
        }

        private string IconFor(string icon)
        {
            return String.Format("/Resources/{0}_{1}.png", icon, (_activeIcon == icon) ? "on" : "off");
        }

        public override void ChangeTile(Tile tile)
        {
            SelectedTile = tile;

            if (SelectedTile != null)
            {
                ConstructTool();
            }
            else
            {
                Tool = null;
                ToolCursor = null;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedTile"));
            }
        }

        private void ConstructTool()
        {
            var brush = new SingleTileBrush(SelectedTile);

            switch (ActiveIcon)
            {
                case "Brush":
                    Tool = new TileBrushToolBehavior(brush);
                    ToolCursor = new SingleTileCursor(_tileset, SelectedTile);
                    break;

                case "Bucket":
                    Tool = new BucketToolBehavior(brush);
                    ToolCursor = new SingleTileCursor(_tileset, SelectedTile);
                    break;

                case "Selection":
                    Tool = new SelectionToolBehavior();
                    ToolCursor = null;
                    break;

                case "Rectangle":
                    Tool = new RectangleToolBehavior(brush);
                    ToolCursor = new SingleTileCursor(_tileset, SelectedTile);
                    break;
            }

            if (ToolChanged != null)
            {
                ToolChanged(this, new ToolChangedEventArgs(_currentTool));
            }
        }

        public TilesetViewModel()
        {
            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);
            ViewModelMediator.Current.GetEvent<TileSelectedEventArgs>().Subscribe((s, e) => ChangeTile(e.Tile));

            ChangeToolCommand = new RelayCommand(ChangeTool, p => HasStage());
            ChangeTool("Brush");
        }

        private void ChangeTool(object toolName)
        {
            ActiveIcon = toolName.ToString();
            ConstructTool();
        }

        private bool HasStage()
        {
            return (_tileset != null);
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            if (e.Stage != null)
                SetStage(e.Stage);
            else
                UnsetStage();
        }

        private void SetStage(StageDocument stage)
        {
            _tileset = stage.Tileset.Tileset;
            ((App)App.Current).AnimateTileset(_tileset);

            ChangeTile(_tileset.FirstOrDefault());

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Tiles"));
            }
        }

        private void UnsetStage()
        {
            _tileset = null;

            ChangeTile(null);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Tiles"));
            }
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
