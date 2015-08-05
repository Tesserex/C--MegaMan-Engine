using MegaMan.Editor.Controls.ViewModels;
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

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                _viewModel = App.Container.Get<AddStageViewModel>();
                this.DataContext = _viewModel;
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {

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
