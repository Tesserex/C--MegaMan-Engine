using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MegaMan.Editor.Controls.ViewModels;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for ProjectTree.xaml
    /// </summary>
    public partial class ProjectTree : UserControl
    {
        private ProjectViewModel _viewModel;

        public ProjectTree()
        {
            InitializeComponent();
        }

        public void Update(ProjectViewModel projectViewModel)
        {
            _viewModel = projectViewModel;

            base.DataContext = _viewModel;
        }

        public static readonly RoutedUICommand ClickCommand = new RoutedUICommand("Click", "Click", typeof(ProjectTree));

        private void StageClick(object sender, ExecutedRoutedEventArgs e)
        {
            var stageInfo = (StageTreeItemViewModel)e.Parameter;

            _viewModel.ChangeStage(stageInfo.StageName);
        }
    }
}
