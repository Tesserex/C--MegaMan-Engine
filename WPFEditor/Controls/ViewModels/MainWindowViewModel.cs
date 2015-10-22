using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using MegaMan.Editor.AppData;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Factories;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Services;
using MegaMan.IO;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IProjectDocumentFactory _projectFactory;
        private IDialogService _dialogService;
        private IDataAccessService _dataService;

        private ProjectDocument _openProject;

        public string ApplicationName { get; private set; }

        public StoredAppData AppData { get; private set; }

        public ProjectViewModel ProjectViewModel { get; private set; }

        public ICommand OpenRecentCommand { get; private set; }
        public ICommand SaveProjectCommand { get; private set; }
        public ICommand CloseProjectCommand { get; private set; }
        public ICommand TestCommand { get; private set; }
        public ICommand TestStageCommand { get; private set; }
        public ICommand TestLocationCommand { get; private set; }
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand EnginePathCommand { get; private set; }
        public ICommand NewEntityCommand { get; private set; }

        private bool _showBackstage;
        public bool ShowBackstage
        {
            get { return _showBackstage; }
            set
            {
                _showBackstage = value;
                OnPropertyChanged("ShowBackstage");
            }
        }

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

        private AvalonDockLayoutViewModel mAVLayout;
        public AvalonDockLayoutViewModel ADLayout
        {
            get
            {
                if (this.mAVLayout == null)
                    this.mAVLayout = new AvalonDockLayoutViewModel();

                return this.mAVLayout;
            }
        }

        public MainWindowViewModel(IProjectDocumentFactory projectFactory, IDialogService dialogService, IDataAccessService dataService)
        {
            _projectFactory = projectFactory;
            _dialogService = dialogService;
            _dataService = dataService;

            ProjectViewModel = new ProjectViewModel();

            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(this.ProjectOpened);

            AppData = StoredAppData.Load();

            var attr = this.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute)).Single() as AssemblyProductAttribute;
            ApplicationName = attr.Product;
            WindowTitle = attr.Product;

            OpenRecentCommand = new RelayCommand(OpenRecentProject, null);
            SaveProjectCommand = new RelayCommand(x => SaveProject(), o => _openProject != null);
            CloseProjectCommand = new RelayCommand(CloseProject, o => _openProject != null);
            TestCommand = new RelayCommand(TestProject, o => _openProject != null);
            TestStageCommand = new RelayCommand(TestStage, o => _openProject != null);
            TestLocationCommand = new RelayCommand(TestLocation, o => _openProject != null);
            UndoCommand = new RelayCommand(Undo, p => ProjectViewModel.CurrentStage != null);
            RedoCommand = new RelayCommand(Redo, p => ProjectViewModel.CurrentStage != null);
            EnginePathCommand = new RelayCommand(ChangeEnginePath);
            NewEntityCommand = new RelayCommand(NewEntity);

            ShowBackstage = true;
        }

        private void NewEntity(object obj)
        {
            _dialogService.ShowNewEntityDialog();
        }

        private void OpenProjectDialog(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Open Project File";
            dialog.FileName = "game";
            dialog.DefaultExt = ".xml";
            dialog.Filter = "XML Files|*.xml";

            var result = dialog.ShowDialog();

            if (result == true)
            {
                TryOpenProject(dialog.FileName);
            }
        }

        private void OpenRecentProject(object param)
        {
            TryOpenProject(param.ToString());
            ShowBackstage = false;
        }

        private void TryOpenProject(string filename)
        {
            try
            {
                OpenProject(filename);
            }
            catch (FileNotFoundException)
            {
                CustomMessageBox.ShowError("The project file could not be found at the specified location.", ApplicationName);

            }
            catch (MegaMan.Common.GameXmlException)
            {
                CustomMessageBox.ShowError("The selected project could not be loaded. There was an error while parsing the project files.", ApplicationName);
            }
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
                _dataService.SaveProject(_openProject);
            }
        }

        public void CloseProject(object arg)
        {
            if (_openProject != null)
            {
                DestroyProjectDependencies();
                _openProject = null;
            }
        }

        public void TestProject(object arg)
        {
            if (_openProject != null)
            {
                var projectPath = Path.Combine(_openProject.Project.BaseDir, "game.xml");
                RunTest(string.Format("\"{0}\"", projectPath));
            }
        }

        private ProcessStartInfo GetEngineStartInfo()
        {
            var enginePath = GetOrFindEnginePath();
            if (enginePath == null)
                return null;

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = enginePath;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.UseShellExecute = false;
            return startInfo;
        }

        public void TestStage(object arg)
        {
            if (_openProject != null && ProjectViewModel.CurrentStage != null)
            {
                var projectPath = Path.Combine(_openProject.Project.BaseDir, "game.xml");
                var stage = ProjectViewModel.CurrentStage.LinkName;

                RunTest(string.Format("\"{0}\" \"STAGE\\{1}\"", projectPath, stage));
            }
        }

        public void TestLocation(object arg)
        {
            if (_openProject != null)
            {

            }
        }

        private void RunTest(string arguments)
        {
            var startInfo = GetEngineStartInfo();
            if (startInfo != null)
            {
                SaveProject();
                startInfo.Arguments = arguments;
                Process.Start(startInfo);
            }
        }

        public void ChangeEnginePath(object arg)
        {
            PromptForEnginePath();
        }

        private void Undo(object param)
        {
            ProjectViewModel.CurrentStage.Undo();
        }

        private void Redo(object param)
        {
            ProjectViewModel.CurrentStage.Redo();
        }

        private string GetOrFindEnginePath()
        {
            if (string.IsNullOrWhiteSpace(AppData.EngineAbsolutePath) || !File.Exists(AppData.EngineAbsolutePath))
                PromptForEnginePath();

            return AppData.EngineAbsolutePath;
        }

        private void PromptForEnginePath()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Executable Files (*.exe)", "exe"));

            dialog.Title = "Please Locate Engine Executable";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                AppData.EngineAbsolutePath = dialog.FileName;
                AppData.Save();
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
