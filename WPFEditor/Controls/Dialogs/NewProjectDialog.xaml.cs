using MegaMan.Editor.Controls.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using Ninject;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for NewProjectDialog.xaml
    /// </summary>
    public partial class NewProjectDialog : UserControl
    {
        private NewProjectViewModel _viewModel;

        public NewProjectDialog()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _viewModel = App.Container.Get<NewProjectViewModel>();
                this.DataContext = _viewModel;
            }
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = _viewModel.DirectoryPath;
            dialog.Title = "Choose Project Location";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _viewModel.DirectoryPath = dialog.FileName;
            }
        }

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Create();
        }
    }
}
