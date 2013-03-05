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

namespace MegaMan.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProjectDocument _openProject;
        private ProjectViewModel _projectViewModel;

        public MainWindow()
        {
            InitializeComponent();

            UseLayoutRounding = true;

            _projectViewModel = new ProjectViewModel();
            projectTree.Update(_projectViewModel);

            var tilesetModel = new TilesetViewModel(_projectViewModel);
            tileStrip.Update(tilesetModel);
            stageTileControl.ToolProvider = tilesetModel;
            stageTileControl.StageProvider = _projectViewModel;

            var layoutEditor = new LayoutEditingViewModel(_projectViewModel);
            layoutToolbar.DataContext = layoutEditor;
            stageLayoutControl.ToolProvider = layoutEditor;
            stageLayoutControl.StageProvider = _projectViewModel;
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
                try
                {
                    _openProject = ProjectDocument.FromFile(dialog.FileName);
                }
                catch (MegaMan.Common.GameXmlException)
                {
                    MessageBox.Show(this, "The selected project could not be loaded. Perhaps it was created with a different version of this editor.",
                        "MegaMan Project Editor", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }
                catch
                {
                    MessageBox.Show(this, "The selected file could not be loaded due to an unknown error.",
                        "MegaMan Project Editor", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                if (_openProject != null)
                {
                    SetupProjectDependencies(_openProject);
                }
            }
        }

        private void SetupProjectDependencies(ProjectDocument project)
        {
            _projectViewModel.Project = project;
        }

        private void DestroyProjectDependencies()
        {
            _projectViewModel.Project = null;
        }

        private void SaveProject(object sender, ExecutedRoutedEventArgs e)
        {
            if (_openProject != null)
            {
                _openProject.Save();
            }
        }

        private void IsProjectOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_openProject != null);
        }

        private void CloseProject(object sender, ExecutedRoutedEventArgs e)
        {
            if (_openProject != null)
            {
                DestroyProjectDependencies();
                _openProject = null;
            }
        }
    }
}
