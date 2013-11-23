using MegaMan.Editor.Controls.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
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

            _viewModel = new NewProjectViewModel();
            this.DataContext = _viewModel;
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
    }
}
