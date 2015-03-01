using System.Windows.Controls;
using System.Windows.Input;
using MegaMan.Editor.Controls;
using MegaMan.Editor.Controls.ViewModels;
using MegaMan.Editor.Mediator;
using Microsoft.Win32;
using Ninject;

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

            OpenRecentCommand = new RelayCommand(OpenRecentProject, null);
            OpenProjectSettingsCommand = new RelayCommand(OpenProjectSettings, p => IsProjectOpen());
            AddStageCommand = new RelayCommand(AddStage, p => IsProjectOpen());
            EditTilesetCommand = new RelayCommand(EditTileset, p => IsStageOpen());
            EditStageCommand = new RelayCommand(EditStage, p => IsStageOpen());

            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageSelected);

            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_viewModel.ProjectViewModel.Project != null && _viewModel.ProjectViewModel.Project.Dirty)
            {
                var result = CustomMessageBox.ShowSavePrompt();
                if (result == System.Windows.MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (result == System.Windows.MessageBoxResult.Yes)
                {
                    _viewModel.ProjectViewModel.Project.Save();
                }
            }
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

        private void RibbonTabChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
