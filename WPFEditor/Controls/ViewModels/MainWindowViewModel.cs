using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MegaMan.Editor.AppData;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Factories;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IProjectDocumentFactory _projectFactory;
        private ProjectDocument _openProject;

        public string ApplicationName { get; private set; }

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

        public IEnumerable<ZoomLevel> ZoomLevels
        {
            get
            {
                yield return ZoomLevel.Half;
                yield return ZoomLevel.Full;
                yield return ZoomLevel.Double;
                yield return ZoomLevel.Triple;
            }
        }

        private ZoomLevel _currentZoom = ZoomLevel.Full;
        public ZoomLevel CurrentZoom
        {
            get { return _currentZoom; }
            set
            {
                _currentZoom = value;
                OnPropertyChanged("CurrentZoom");
                ViewModelMediator.Current.GetEvent<ZoomChangedEventArgs>().Raise(this, new ZoomChangedEventArgs() { Zoom = _currentZoom.Zoom });
                App.Current.Resources["Zoom"] = _currentZoom.Zoom;
            }
        }

        public MainWindowViewModel(IProjectDocumentFactory projectFactory)
        {
            _projectFactory = projectFactory;

            ProjectViewModel = new ProjectViewModel();

            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(this.ProjectOpened);

            AppData = StoredAppData.Load();

            var attr = this.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute)).Single() as AssemblyProductAttribute;
            ApplicationName = attr.Product;
            WindowTitle = attr.Product;
        }

        public void OpenProject(string filename)
        {
            var project = _projectFactory.Load(filename);

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
            WindowTitle = project.Name + " - " + ApplicationName;
        }

        private void DestroyProjectDependencies()
        {
            ProjectViewModel.Project = null;
            WindowTitle = ApplicationName;
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
