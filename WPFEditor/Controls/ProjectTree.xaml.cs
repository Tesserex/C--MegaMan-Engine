using System.Windows.Controls;
using System.Windows.Input;
using MegaMan.Editor.Controls.ViewModels;

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
            stageInfo.IsSelected = true;
            _viewModel.ChangeStage(stageInfo.StageName);
        }
    }
}
