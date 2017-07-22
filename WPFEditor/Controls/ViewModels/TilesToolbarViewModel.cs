using System;
using System.Windows;
using System.Windows.Input;
using MegaMan.Editor.Bll.Tools;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Tools;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class TilesToolbarViewModel : ViewModelBase, IToolProvider
    {
        private ITileBrush _currentBrush;
        private IToolBehavior _currentTool;
        private IToolCursor _currentCursor;
        private string _activeIcon;
        private bool _bucketGlobal;

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
                OnPropertyChanged("BucketOptionVisibility");
            }
        }

        private string IconFor(string icon)
        {
            return String.Format("/Resources/{0}_{1}.png", icon, (_activeIcon == icon) ? "on" : "off");
        }

        private void ChangeBrush(ITileBrush brush)
        {
            _currentBrush = brush;
            ConstructTool();
        }

        private void ConstructTool()
        {
            if (_currentBrush == null)
                return;

            switch (ActiveIcon)
            {
                case "Brush":
                    Tool = new TileBrushToolBehavior(_currentBrush);
                    ToolCursor = new TileBrushCursor(_currentBrush);
                    break;

                case "Bucket":
                    Tool = new BucketToolBehavior(_currentBrush) { IsGlobal = BucketModeGlobal };
                    ToolCursor = new TileBrushCursor(_currentBrush);
                    break;

                case "Selection":
                    Tool = new SelectionToolBehavior();
                    ToolCursor = null;
                    break;

                case "Rectangle":
                    Tool = new RectangleToolBehavior(_currentBrush);
                    ToolCursor = new TileBrushCursor(_currentBrush);
                    break;
            }

            if (ToolChanged != null)
            {
                ToolChanged(this, new ToolChangedEventArgs(_currentTool));
            }
        }

        public TilesToolbarViewModel()
        {
            ViewModelMediator.Current.GetEvent<TileBrushSelectedEventArgs>().Subscribe((s, e) => ChangeBrush(e.TileBrush));
            ChangeToolCommand = new RelayCommand(ChangeTool);

            ChangeTool("Brush");
        }

        private void ChangeTool(object toolName)
        {
            ActiveIcon = toolName.ToString();
            ConstructTool();
        }

        public bool BucketModeGlobal
        {
            get { return _bucketGlobal; }
            set
            {
                _bucketGlobal = value;
                if (Tool is BucketToolBehavior)
                {
                    ((BucketToolBehavior)Tool).IsGlobal = value;
                }
                OnPropertyChanged();
            }
        }

        public Visibility BucketOptionVisibility
        {
            get { return (ActiveIcon == "Bucket") ? Visibility.Visible : Visibility.Collapsed; }
        }
    }
}
