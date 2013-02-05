using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Editor.Bll;
using MegaMan.Common;
using MegaMan.Editor.Bll.Tools;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class TilesetViewModel : IToolProvider, IRequireCurrentStage, INotifyPropertyChanged
    {
        private Tileset _tileset;
        private IToolBehavior _currentTool;

        public IToolBehavior Tool
        {
            get { return _currentTool; }
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

        public void SetStage(StageDocument stage)
        {
            _tileset = stage.Tileset;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Tiles"));
            }
        }

        public void ChangeTile(Tile tile)
        {
            
        }

        public IEnumerable<Tile> Tiles
        {
            get
            {
                return _tileset;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
