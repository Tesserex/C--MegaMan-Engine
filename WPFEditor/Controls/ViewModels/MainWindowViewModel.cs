using MegaMan.Editor.AppData;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ProjectDocument _openProject;

        public StoredAppData AppData { get; private set; }

        public ProjectViewModel ProjectViewModel { get; private set; }

        private string _windowTitle;
        public string WindowTitle
        {
            get
            {
                return _windowTitle;
            }
            set
            {
                _windowTitle = value;
                OnPropertyChanged("WindowTitle");
            }
        }

        public MainWindowViewModel()
        {
            ProjectViewModel = new ProjectViewModel();

            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(this.ProjectOpened);

            AppData = StoredAppData.Load();

            WindowTitle = "Mega Man Project Editor";
        }

        public void OpenProject(string filename)
        {
            var project = ProjectDocument.FromFile(filename);

            if (project != null)
            {
                var args = new ProjectOpenedEventArgs() { Project = project };
                ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Raise(this, args);
            }
        }

        private void ProjectOpened(object sender, ProjectOpenedEventArgs args)
        {
            SetupProjectDependencies(args.Project);

            AppData.AddRecentProject(args.Project);
            AppData.Save();
        }

        private void SetupProjectDependencies(ProjectDocument project)
        {
            _openProject = project;
            ProjectViewModel.Project = project;
            WindowTitle = project.Name + " - Mega Man Project Editor";
        }

        private void DestroyProjectDependencies()
        {
            ProjectViewModel.Project = null;
        }

        public void SaveProject()
        {
            if (_openProject != null)
            {
                _openProject.Save();
            }
        }

        public void CloseProject()
        {
            if (_openProject != null)
            {
                DestroyProjectDependencies();
                _openProject = null;
            }
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
