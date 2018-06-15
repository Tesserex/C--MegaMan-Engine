using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Controls;
using MegaMan.Editor.Controls.ViewModels;
using MegaMan.Editor.Controls.ViewModels.Dialogs;
using MegaMan.Editor.Mediator;
using Microsoft.WindowsAPICodePack.Dialogs;
using Ninject;
using SelectionChangedEventArgs = System.Windows.Controls.SelectionChangedEventArgs;

namespace MegaMan.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainWindowViewModel _viewModel;

        public ICommand OpenProjectSettingsCommand { get; private set; }
        public ICommand EditTilesetCommand { get; private set; }
        public ICommand EditStageCommand { get; private set; }
        public ICommand AddStageCommand { get; private set; }
        public ICommand LinkStageCommand { get; private set; }
        public ICommand StagePropertiesCommand { get; private set; }
        public ICommand ImportTilesCommand { get; private set; }

        public MainWindow()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _viewModel = App.Container.Get<MainWindowViewModel>();
                DataContext = _viewModel;
            }

            UseLayoutRounding = true;

            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                projectTree.Update(_viewModel.ProjectViewModel);

                OpenProjectSettingsCommand = new RelayCommand(OpenProjectSettings, p => IsProjectOpen());
                AddStageCommand = new RelayCommand(AddStage, p => IsProjectOpen());
                LinkStageCommand = new RelayCommand(LinkStage, p => IsProjectOpen());
                EditTilesetCommand = new RelayCommand(EditTileset, p => IsStageOpen());
                EditStageCommand = new RelayCommand(EditStage, p => IsStageOpen());
                StagePropertiesCommand = new RelayCommand(ShowStageProperties, p => IsStageOpen());
                ImportTilesCommand = new RelayCommand(ImportTiles);

                ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageSelected);
                ViewModelMediator.Current.GetEvent<EntitySelectedEventArgs>().Subscribe(EntitySelected);

                Closing += MainWindow_Closing;
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_viewModel.CurrentProject != null && _viewModel.CurrentProject.Dirty)
            {
                var result = CustomMessageBox.ShowSavePrompt();
                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (result == MessageBoxResult.Yes)
                {
                    _viewModel.SaveProject();
                }
            }
        }

        private void StageSelected(object sender, StageChangedEventArgs e)
        {
            if (e.Stage != null)
                ribbonStage.IsSelected = true;

            editorPane.IsActive = true;
        }

        private void EntitySelected(object sender, EntitySelectedEventArgs e)
        {
            if (e.Entity != null)
                ribbonEntities.IsSelected = true;

            entityEditorPane.IsActive = true;
        }

        private bool IsProjectOpen()
        {
            return (_viewModel.CurrentProject != null);
        }

        private void OpenProjectSettings(object param)
        {
            settingsControl.DataContext = new ProjectSettingsViewModel(_viewModel.CurrentProject);
            projectSettingsPane.IsSelected = true;
        }

        private void ShowStageProperties(object obj)
        {
            stagePropertiesPane.IsSelected = true;
        }

        private bool IsStageOpen()
        {
            return IsProjectOpen() && _viewModel.ProjectViewModel.CurrentStage != null;
        }

        private void EditTileset(object param)
        {
            tilesetEditorPane.IsSelected = true;
        }

        private void ImportTiles(object param)
        {
            tilesetImporterControl.DataContext = new TilesetImporterViewModel((TilesetDocument)param);
            tilesetImporterPane.IsSelected = true;
        }

        private void AddStage(object param)
        {
            addStagePane.IsSelected = true;
        }

        private void LinkStage(object param)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = _viewModel.CurrentProject.Project.BaseDir;
            dialog.Title = "Select Stage Folder";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _viewModel.CurrentProject.LinkStage(dialog.FileName);
            }
        }

        private void RibbonTabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Contains(ribbonStage))
                editorPane.IsSelected = true;
            else if (e.AddedItems.Contains(ribbonEntities))
                entityEditorPane.IsSelected = true;
        }

        private void EditStage(object param)
        {
            editorPane.IsSelected = true;
        }
    }
}
