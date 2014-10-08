using MegaMan.Editor.Controls;
using MegaMan.Editor.Controls.ViewModels;
using MegaMan.Editor.Mediator;
using Microsoft.Win32;
using Ninject;
using System.Windows.Controls;
using System.Windows.Input;

namespace MegaMan.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainWindowViewModel _viewModel;

        public ICommand OpenRecentCommand { get; private set; }
        public ICommand OpenProjectSettingsCommand { get; private set; }
        public ICommand EditTilesetCommand { get; private set; }
        public ICommand EditStageCommand { get; private set; }
        public ICommand AddStageCommand { get; private set; }

        public MainWindow()
        {
            _viewModel = App.Container.Get<MainWindowViewModel>();
            this.DataContext = _viewModel;

            UseLayoutRounding = true;

            InitializeComponent();

            projectTree.Update(_viewModel.ProjectViewModel);

            var tilesetModel = new TilesetViewModel();
            tileStrip.Update(tilesetModel);
            stageTileControl.ToolProvider = tilesetModel;

            var layoutEditor = new LayoutEditingViewModel();
            layoutToolbar.DataContext = layoutEditor;
            stageLayoutControl.ToolProvider = layoutEditor;

            OpenRecentCommand = new RelayCommand(OpenRecentProject, null);
            OpenProjectSettingsCommand = new RelayCommand(OpenProjectSettings, p => IsProjectOpen());
            AddStageCommand = new RelayCommand(AddStage, p => IsProjectOpen());
            EditTilesetCommand = new RelayCommand(EditTileset, p => IsStageOpen());
            EditStageCommand = new RelayCommand(EditStage, p => IsStageOpen());

            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageSelected);
        }

        private void StageSelected(object sender, StageChangedEventArgs e)
        {
            if (e.Stage != null)
                ribbonStage.IsSelected = true;

            this.editorPane.IsActive = true;
        }

        private void CanExecuteTrue(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenProject(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Open Project File";
            dialog.FileName = "game";
            dialog.DefaultExt = ".xml";
            dialog.Filter = "XML Files|*.xml";

            var result = dialog.ShowDialog(this);

            if (result == true)
            {
                TryOpenProject(dialog.FileName);
            }
        }

        private void TryOpenProject(string filename)
        {
            try
            {
                _viewModel.OpenProject(filename);
            }
            catch (System.IO.FileNotFoundException)
            {
                CustomMessageBox.ShowError("The project file could not be found at the specified location.", _viewModel.ApplicationName);

            }
            catch (MegaMan.Common.GameXmlException)
            {
                CustomMessageBox.ShowError("The selected project could not be loaded. There was an error while parsing the project files.", _viewModel.ApplicationName);
            }
        }

        private void SaveProject(object sender, ExecutedRoutedEventArgs e)
        {
            _viewModel.SaveProject();
        }

        private void IsProjectOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectOpen();
        }

        private bool IsProjectOpen()
        {
            return (_viewModel.ProjectViewModel.Project != null);
        }

        private void CloseProject(object sender, ExecutedRoutedEventArgs e)
        {
            _viewModel.CloseProject();
        }

        private void OpenRecentProject(object param)
        {
            TryOpenProject(param.ToString());
            ribbonBackstage.IsOpen = false;
        }

        private void OpenProjectSettings(object param)
        {
            this.settingsControl.DataContext = new ProjectSettingsViewModel(_viewModel.ProjectViewModel.Project);
            this.projectSettingsPane.IsSelected = true;
        }

        private bool IsStageOpen()
        {
            return IsProjectOpen() && _viewModel.ProjectViewModel.CurrentStage != null;
        }

        private void EditTileset(object param)
        {
            this.tilesetEditorPane.IsSelected = true;
        }

        private void AddStage(object param)
        {
            this.addStagePane.IsSelected = true;
        }

        private void RibbonTabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Contains(ribbonStage))
                editorPane.IsSelected = true;
        }

        private void EditStage(object param)
        {
            editorPane.IsSelected = true;
        }
    }
}
