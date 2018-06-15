using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MegaMan.Editor.Controls.ViewModels
{
    class ProjectSettingsViewModel : INotifyPropertyChanged
    {
        private ProjectDocument _project;

        public string Name
        {
            get { return _project.Name; }
            set
            {
                _project.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Author
        {
            get { return _project.Author; }
            set
            {
                _project.Author = value;
                OnPropertyChanged("Author");
            }
        }

        public string MusicNsf
        {
            get { return _project.MusicNsf; }
            set
            {
                _project.MusicNsf = value;
                OnPropertyChanged("MusicNsf");
            }
        }

        public string EffectsNsf
        {
            get { return _project.EffectsNsf; }
            set
            {
                _project.EffectsNsf = value;
                OnPropertyChanged("EffectsNsf");
            }
        }

        public IEnumerable<HandlerType> HandlerTypes
        {
            get
            {
                return Enum.GetValues(typeof(HandlerType))
                    .Cast<HandlerType>()
                    .OrderBy(t => t.ToString());
            }
        }

        public HandlerType StartType
        {
            get { return _project.StartHandlerType; }
            set
            {
                _project.StartHandlerType = value;
                OnPropertyChanged("StartType");
                OnPropertyChanged("StartHandlers");

                StartName = StartHandlers.FirstOrDefault();
            }
        }

        public IEnumerable<string> StartHandlers
        {
            get
            {
                var items = Enumerable.Empty<string>();

                if (StartType == HandlerType.Stage)
                    items = _project.StageNames;
                else if (StartType == HandlerType.Scene)
                    items = _project.SceneNames;
                else if (StartType == HandlerType.Menu)
                    items = _project.MenuNames;

                return items.OrderBy(x => x);
            }
        }

        public string StartName
        {
            get { return _project.StartHandlerName; }
            set
            {
                _project.StartHandlerName = value;
                OnPropertyChanged("StartName");
            }
        }

        public IEnumerable<string> IncludeFiles
        {
            get
            {
                return _project.Project.IncludeFolders.Concat(_project.Project.IncludeFiles).Select(p => p.Absolute);
            }
        }

        private string _selectedFile;
        public string SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                OnPropertyChanged("SelectedFile");
            }
        }

        public ICommand AddIncludeFolderCommand { get; set; }
        public ICommand AddIncludeFileCommand { get; set; }
        public ICommand RemoveIncludeCommand { get; set; }

        public ProjectSettingsViewModel(ProjectDocument project)
        {
            SetProject(project);
            ViewModelMediator.Current.GetEvent<ProjectChangedEventArgs>().Subscribe((s, e) => SetProject(e.Project));

            AddIncludeFolderCommand = new RelayCommand(AddIncludeFolder, o => _project != null);
            AddIncludeFileCommand = new RelayCommand(AddIncludeFile, o => _project != null);
            RemoveIncludeCommand = new RelayCommand(RemoveInclude, o => SelectedFile != null);
        }

        private void AddIncludeFolder(object obj)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = _project.Project.BaseDir;
            dialog.Title = "Select a Folder";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _project.Project.AddIncludeFolder(dialog.FileName);
                OnPropertyChanged("IncludeFiles");
            }
        }

        private void AddIncludeFile(object obj)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            dialog.InitialDirectory = _project.Project.BaseDir;
            dialog.Title = "Select a File";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = true;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _project.Project.AddIncludeFiles(dialog.FileNames);
                OnPropertyChanged("IncludeFiles");
            }
        }

        private void RemoveInclude(object obj)
        {
            if (SelectedFile != null)
            {
                _project.Project.RemoveInclude(SelectedFile);
                OnPropertyChanged("IncludeFiles");
            }
        }

        public void SetProject(ProjectDocument projectDocument)
        {
            _project = projectDocument;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
