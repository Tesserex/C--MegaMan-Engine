using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Tools;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Tools;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class TileBrushControlViewModel : INotifyPropertyChanged, IToolProvider
    {
        private TilesetDocument _tileset;

        private ScreenDocument _selectionScreen;
        private Rectangle? _selection;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddTileBrushCommand { get; private set; }
        public ICommand CreateBrushSelectionCommand { get; private set; }

        private ObservableCollection<MultiTileBrush> _observedBrushes;

        public IEnumerable<MultiTileBrush> Brushes
        {
            get
            {
                return _observedBrushes;
            }
        }

        private void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public TileBrushControlViewModel()
        {
            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);
            ViewModelMediator.Current.GetEvent<SelectionChangedEventArgs>().Subscribe(SelectionChanged);

            AddTileBrushCommand = new RelayCommand(AddTileBrush, o => _tileset != null);
            CreateBrushSelectionCommand = new RelayCommand(CreateSelectionBrush, o => _selection != null);
        }

        private void CreateSelectionBrush(object obj)
        {
            if (_selection == null)
                return;

            var s = _selection.Value;

            var brush = new MultiTileBrush(s.Width, s.Height);
            for (var x = 0; x < s.Width; x++)
            {
                for (var y = 0; y < s.Height; y++)
                {
                    brush.AddTile(_selectionScreen.TileAt(x + s.X, y + s.Y), x, y);
                }
            }

            _tileset.Brushes.Add(brush);
            _observedBrushes.Add(brush);
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Selection != null)
            {
                _selection = e.Selection;
                _selectionScreen = e.Screen;
            }
            else if (e.Screen == _selectionScreen)
            {
                _selection = null;
            }
        }

        private void AddTileBrush(object obj)
        {

        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            if (e.Stage != null)
                SetTileset(e.Stage.Tileset);
            else
                SetTileset(null);
        }

        private void SetTileset(TilesetDocument tileset)
        {
            _tileset = tileset;

            if (tileset == null)
            {
                _observedBrushes = new ObservableCollection<MultiTileBrush>();
            }
            else
            {
                _observedBrushes = new ObservableCollection<MultiTileBrush>(_tileset.Brushes);
            }

            OnPropertyChanged("Brushes");
        }

        internal void SelectBrush(MultiTileBrush multiTileBrush)
        {
            Tool = new TileBrushToolBehavior(multiTileBrush);
            ToolCursor = new MultiTileCursor(multiTileBrush);

            if (ToolChanged != null)
            {
                ToolChanged(this, new ToolChangedEventArgs(Tool));
            }
        }

        public IToolBehavior Tool
        {
            get;
            private set;
        }

        public IToolCursor ToolCursor
        {
            get;
            private set;
        }

        public event System.EventHandler<ToolChangedEventArgs> ToolChanged;
    }
}
