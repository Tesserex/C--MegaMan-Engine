using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MegaMan.Editor.Controls.ViewModels;
using Ninject;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for AddStageDialog.xaml
    /// </summary>
    public partial class AddStageDialog : UserControl
    {
        private AddStageViewModel _viewModel;

        public AddStageDialog()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _viewModel = App.Container.Get<AddStageViewModel>();
                DataContext = _viewModel;
            }
        }

        private void ClickCreateTileset(object sender, RoutedEventArgs e)
        {
            _viewModel.CreateTileset = true;
        }

        private void ClickExistingTileset(object sender, RoutedEventArgs e)
        {
            _viewModel.ExistingTileset = true;
        }
    }
}
