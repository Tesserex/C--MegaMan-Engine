using System.Windows;
using System.Windows.Controls;
using MegaMan.Editor.Controls.ViewModels;
using Microsoft.Win32;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for ProjectSettingsControl.xaml
    /// </summary>
    public partial class ProjectSettingsControl : UserControl
    {
        public ProjectSettingsControl()
        {
            InitializeComponent();
        }

        private void BrowseMusicClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ProjectSettingsViewModel;

            if (vm == null)
            {
                CustomMessageBox.ShowError("The settings screen failed to initialize properly.", "My plan has been foild!");
                return; // should never happen
            }

            var dialog = new OpenFileDialog();
            dialog.Title = "Choose Music NSF File";
            dialog.DefaultExt = ".nsf";
            dialog.Filter = "NSF Files|*.nsf";

            dialog.FileName = vm.MusicNsf;

            var parentWindow = Window.GetWindow(this);
            var result = dialog.ShowDialog(parentWindow);

            if (result == true)
                vm.MusicNsf = dialog.FileName;
        }
    }
}
