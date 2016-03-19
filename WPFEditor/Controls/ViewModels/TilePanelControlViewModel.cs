using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Tools;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class TilePanelControlViewModel : TilesetViewModelBase
    {
        private ScreenDocument _selectionScreen;
        private Rectangle? _selection;

        public ICommand AddTileBrushCommand { get; private set; }
        public ICommand CreateBrushSelectionCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }

        private ObservableCollection<MultiTileBrush> _observedBrushes;

        public int Zoom { get; private set; }

        public IEnumerable<MultiTileBrush> Brushes
        {
            get
            {
                return _observedBrushes;
            }
        }

        public MultiTileBrush SelectedBrush { get; set; }

        public TilePanelControlViewModel()
        {
            ViewModelMediator.Current.GetEvent<SelectionChangedEventArgs>().Subscribe(SelectionChanged);

            Zoom = 1;
            AddTileBrushCommand = new RelayCommand(AddTileBrush, o => _tileset != null);
            CreateBrushSelectionCommand = new RelayCommand(CreateSelectionBrush, o => _selection != null);
            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
        }

        private void ZoomIn(object arg)
        {
            if (Zoom < 4)
            {
                Zoom++;
                OnPropertyChanged("Zoom");
            }
        }

        private void ZoomOut(object arg)
        {
            if (Zoom > 1)
            {
                Zoom--;
                OnPropertyChanged("Zoom");
            }
        }

        private bool _ignoreTileChanged;

        public override void ChangeTile(Tile tile)
        {
            if (_ignoreTileChanged)
                return;

            SelectedTile = tile;

            // prevent infinite recursion
            _ignoreTileChanged = true;
            var args = new TileBrushSelectedEventArgs() { TileBrush = tile != null ? new SingleTileBrush(tile) : null };
            ViewModelMediator.Current.GetEvent<TileBrushSelectedEventArgs>().Raise(this, args);
            _ignoreTileChanged = false;

            SelectedBrush = null;
            OnPropertyChanged("SelectedTile");
            OnPropertyChanged("SelectedBrush");
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

            _tileset.AddBrush(brush);
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

        protected override void SetTileset(TilesetDocument tileset)
        {
            base.SetTileset(tileset);

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
            var args = new TileBrushSelectedEventArgs() { TileBrush = multiTileBrush };
            ViewModelMediator.Current.GetEvent<TileBrushSelectedEventArgs>().Raise(this, args);

            SelectedBrush = multiTileBrush;
            OnPropertyChanged("SelectedBrush");
        }
    }
}
