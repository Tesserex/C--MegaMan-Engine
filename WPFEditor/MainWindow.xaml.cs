using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Controls;
using MegaMan.Editor.Controls.ViewModels;
using MegaMan.Editor.Controls.ViewModels.Dialogs;
using MegaMan.Editor.Mediator;
using Microsoft.WindowsAPICodePack.Dialogs;
using Ninject;

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
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                _viewModel = App.Container.Get<MainWindowViewModel>();
                this.DataContext = _viewModel;
            }

            UseLayoutRounding = true;

            InitializeComponent();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
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

                this.Closing += MainWindow_Closing;
            }
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
                    _viewModel.SaveProject();
                }
            }
        }

        private void StageSelected(object sender, StageChangedEventArgs e)
        {
            if (e.Stage != null)
                ribbonStage.IsSelected = true;

            this.editorPane.IsActive = true;
        }

        private void EntitySelected(object sender, EntitySelectedEventArgs e)
        {
            if (e.Entity != null)
                ribbonEntities.IsSelected = true;

            this.entityEditorPane.IsActive = true;
        }

        private bool IsProjectOpen()
        {
            return (_viewModel.ProjectViewModel.Project != null);
        }

        private void OpenProjectSettings(object param)
        {
            this.settingsControl.DataContext = new ProjectSettingsViewModel(_viewModel.ProjectViewModel.Project);
            this.projectSettingsPane.IsSelected = true;
        }

        private void ShowStageProperties(object obj)
        {
            this.stagePropertiesPane.IsSelected = true;
        }

        private bool IsStageOpen()
        {
            return IsProjectOpen() && _viewModel.ProjectViewModel.CurrentStage != null;
        }

        private void EditTileset(object param)
        {
            this.tilesetEditorPane.IsSelected = true;
        }

        private void ImportTiles(object param)
        {
            this.tilesetImporterControl.DataContext = new TilesetImporterViewModel((TilesetDocument)param);
            this.tilesetImporterPane.IsSelected = true;
        }

        private void AddStage(object param)
        {
            this.addStagePane.IsSelected = true;
        }

        private void LinkStage(object param)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = _viewModel.ProjectViewModel.Project.Project.BaseDir;
            dialog.Title = "Select Stage Folder";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _viewModel.ProjectViewModel.Project.LinkStage(dialog.FileName);
            }
        }

        private void RibbonTabChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
