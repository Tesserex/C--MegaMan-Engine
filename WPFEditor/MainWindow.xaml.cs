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

        private List<IRequireCurrentStage> _stageDependents;

        public MainWindow()
        {
            InitializeComponent();

            UseLayoutRounding = true;

            _stageDependents = new List<IRequireCurrentStage>();

            _stageDependents.Add(new Animator());
            _stageDependents.Add(stageLayoutControl);
            _stageDependents.Add(stageTileControl);

            var tilesetModel = new TilesetViewModel();
            _stageDependents.Add(tilesetModel);
            tileStrip.Update(tilesetModel);
            stageTileControl.ToolProvider = tilesetModel;
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
            if (_projectViewModel != null)
            {
                _projectViewModel.StageChanged -= StageChanged;
            }

            _projectViewModel = new ProjectViewModel(project);
            projectTree.Update(_projectViewModel);

            _projectViewModel.StageChanged += StageChanged;

            layoutToolbar.DataContext = new LayoutEditingViewModel(_projectViewModel);
        }

        private void DestroyProjectDependencies()
        {
            if (_projectViewModel != null)
            {
                _projectViewModel.StageChanged -= StageChanged;
            }

            _projectViewModel = null;
            projectTree.Update(null);

            foreach (var dependent in _stageDependents)
            {
                dependent.UnsetStage();
            }

            layoutToolbar.DataContext = null;
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            ChangeStage(e.Stage);
        }

        private void ChangeStage(StageDocument stageDocument)
        {
            foreach (var dependent in _stageDependents)
            {
                dependent.SetStage(stageDocument);
            }
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
