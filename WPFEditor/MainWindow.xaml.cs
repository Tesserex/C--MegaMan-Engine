using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MegaMan.Editor.Bll;
using Microsoft.Win32;
using MegaMan.Editor.Controls.ViewModels;
using MegaMan.Editor.Mediator;
using Fluent;
using MegaMan.Editor.AppData;
using MegaMan.Editor.Controls;

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

        public MainWindow()
        {
            _viewModel = new MainWindowViewModel();
            this.DataContext = _viewModel;

            UseLayoutRounding = true;

            InitializeComponent();

            projectTree.Update(_viewModel.ProjectViewModel);

            var tilesetModel = new TilesetViewModel(_viewModel.ProjectViewModel);
            tileStrip.Update(tilesetModel);
            stageTileControl.ToolProvider = tilesetModel;
            stageTileControl.StageProvider = _viewModel.ProjectViewModel;

            var layoutEditor = new LayoutEditingViewModel(_viewModel.ProjectViewModel);
            layoutToolbar.DataContext = layoutEditor;
            stageLayoutControl.ToolProvider = layoutEditor;
            stageLayoutControl.StageProvider = _viewModel.ProjectViewModel;

            OpenRecentCommand = new RelayCommand(OpenRecentProject, null);
            OpenProjectSettingsCommand = new RelayCommand(OpenProjectSettings, p => IsProjectOpen());
        }

        private void CanExecuteTrue(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewProject(object sender, ExecutedRoutedEventArgs e)
        {

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
            catch
            {
                CustomMessageBox.ShowError("The selected file could not be loaded due to an unknown error.", _viewModel.ApplicationName);
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
            this.projectSettingsPane.IsActive = true;
        }
    }
}
