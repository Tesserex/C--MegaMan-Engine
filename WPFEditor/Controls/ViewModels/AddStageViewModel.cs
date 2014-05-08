using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.ComponentModel;
using System.Windows.Input;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class AddStageViewModel : INotifyPropertyChanged
    {
        private ProjectDocument _project;

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _tilesetPath;
        public string TilesetPath
        {
            get { return _tilesetPath; }
            set
            {
                _tilesetPath = value;
                OnPropertyChanged("TilesetPath");
            }
        }

        private string _tilesheetPath;
        public string TilesheetPath
        {
            get { return _tilesheetPath; }
            set
            {
                _tilesheetPath = value;
                OnPropertyChanged("TilesheetPath");
            }
        }

        private bool _createTileset;
        public bool CreateTileset
        {
            get { return _createTileset; }
            set
            {
                _createTileset = value;
                _existingTileset = !value;
                OnPropertyChanged("CreateTileset");
                OnPropertyChanged("ExistingTileset");
            }
        }

        private bool _existingTileset;
        public bool ExistingTileset
        {
            get { return _existingTileset; }
            set
            {
                _existingTileset = value;
                _createTileset = !value;
                OnPropertyChanged("ExistingTileset");
                OnPropertyChanged("CreateTileset");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddStageCommand { get; set; }
        public ICommand BrowseTilesetCommand { get; set; }
        public ICommand BrowseTilesheetCommand { get; set; }

        private void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public AddStageViewModel()
        {
            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(ProjectChanged);

            AddStageCommand = new RelayCommand(AddStage);
            BrowseTilesetCommand = new RelayCommand(BrowseTileset);
            BrowseTilesheetCommand = new RelayCommand(BrowseTilesheet);

            CreateTileset = true;
        }

        private void BrowseTileset(object obj)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Tilesets (*.xml)", "xml"));

            if (TilesetPath != null)
                dialog.InitialDirectory = TilesetPath;
            else
                dialog.InitialDirectory = _project.Project.BaseDir;

            dialog.Title = "Choose Tileset";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                TilesetPath = dialog.FileName;
            }
        }

        private void BrowseTilesheet(object obj)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Images (*.png, *.gif, *.bmp)", "png,gif,bmp"));

            if (TilesheetPath != null)
                dialog.InitialDirectory = TilesheetPath;
            else
                dialog.InitialDirectory = _project.Project.BaseDir;

            dialog.Title = "Choose Tilesheet Image";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                TilesheetPath = dialog.FileName;
            }
        }

        private void AddStage(object obj)
        {
            if (ExistingTileset)
            {
                if (!System.IO.File.Exists(TilesetPath))
                {
                    CustomMessageBox.ShowError("That tileset file does not exist.", "Foild!");
                    return;
                }

                var stage = _project.AddStage(Name);
                stage.ChangeTileset(TilesetPath);
                _project.Save();
            }
            else
            {
                if (!System.IO.File.Exists(TilesheetPath))
                {
                    CustomMessageBox.ShowError("That image file does not exist.", "Foild!");
                    return;
                }

                var stage = _project.AddStage(Name);
                stage.CreateTileset(TilesheetPath);
                _project.Save();
            }
        }

        private void ProjectChanged(object sender, ProjectOpenedEventArgs e)
        {
            _project = e.Project;
        }
    }
}
