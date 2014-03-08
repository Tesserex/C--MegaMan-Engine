using MegaMan.Common;
using MegaMan.Editor.Bll;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class TilesetEditorViewModel : TilesetViewModelBase, INotifyPropertyChanged
    {
        private ProjectDocument _project;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public ICommand ChangeSheetCommand { get; private set; }

        public SpriteEditorViewModel Sprite { get; private set; }

        public string RelSheetPath
        {
            get
            {
                return _tileset.SheetPath.Relative;
            }
        }

        public TilesetEditorViewModel(Tileset tileset, ProjectDocument project)
        {
            _tileset = tileset;
            _project = project;

            Sprite = new SpriteEditorViewModel(_tileset.First().Sprite);
            OnPropertyChanged("Sprite");

            ChangeSheetCommand = new RelayCommand(o => ChangeSheet());

            if (!File.Exists(_tileset.SheetPath.Absolute))
            {
                ChangeSheet();
            }
        }

        private void ChangeSheet()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Images", "png,gif,jpg,jpeg,bmp"));

            if (_tileset.SheetPath != null)
                dialog.InitialDirectory = _tileset.SheetPath.Absolute;
            else
                dialog.InitialDirectory = _project.Project.BaseDir;

            dialog.Title = "Choose Tileset Image";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _tileset.ChangeSheetPath(dialog.FileName);
                OnPropertyChanged("SheetPath");
                OnPropertyChanged("RelSheetPath");

                Sprite.ChangeSprite(_tileset.First().Sprite);
            }
        }

        public override void ChangeTile(Tile tile)
        {
            SelectedTile = tile;
            Sprite.ChangeSprite(tile.Sprite);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedTile"));
            }
        }
    }
}
