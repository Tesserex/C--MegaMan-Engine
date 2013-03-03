using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Editor.Bll;
using MegaMan.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MegaMan.Editor.Tools;
using MegaMan.Editor.Bll.Tools;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class TilesetViewModel : IToolProvider, IRequireCurrentStage, INotifyPropertyChanged
    {
        private Tileset _tileset;
        private IToolBehavior _currentTool;
        private IToolCursor _currentCursor;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ToolChangedEventArgs> ToolChanged;

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

        public string SheetPath
        {
            get
            {
                if (_tileset != null)
                {
                    return _tileset.SheetPath.Absolute;
                }
                else
                {
                    return null;
                }
            }
        }

        public IEnumerable<Tile> Tiles
        {
            get
            {
                return _tileset;
            }
        }

        public Tile SelectedTile { get; private set; }

        public void SetStage(StageDocument stage)
        {
            _tileset = stage.Tileset;

            ChangeTile(_tileset.FirstOrDefault());

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Tiles"));
            }
        }

        public void UnsetStage()
        {
            _tileset = null;

            ChangeTile(null);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Tiles"));
            }
        }

        public void ChangeTile(Tile tile)
        {
            if (tile != null)
            {
                Tool = new TileBrushToolBehavior(new SingleTileBrush(tile));
                ToolCursor = new SingleTileCursor(_tileset, tile);
            }
            else
            {
                Tool = null;
                ToolCursor = null;
            }

            SelectedTile = tile;

            if (ToolChanged != null)
            {
                ToolChanged(this, new ToolChangedEventArgs(_currentTool));
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedTile"));
            }
        }   
    }
}
