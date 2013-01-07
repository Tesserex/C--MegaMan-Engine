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

namespace MegaMan.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProjectDocument _openProject;
        private Animator _animator;

        public MainWindow()
        {
            InitializeComponent();

            UseLayoutRounding = true;

            _animator = new Animator();

            projectTree.StageSelected += projectTree_StageDoubleClicked;
        }

        private void projectTree_StageDoubleClicked(string stageName)
        {
            if (_openProject != null)
            {
                stageLayoutControl.Stage = _openProject.StageByName(stageName);
                _animator.ChangeTileset(stageLayoutControl.Stage.Tileset);
            }
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
                    projectTree.Update(_openProject.Project);
                }
            }
        }
    }
}
