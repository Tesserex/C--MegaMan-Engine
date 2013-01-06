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
using Microsoft.Win32;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                MegaMan.Common.Project project;

                try
                {
                    project = new MegaMan.Common.Project();
                    project.Load(dialog.FileName);
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

                if (project != null)
                {
                    projectTree.Update(project);
                }
            }
        }
    }
}
