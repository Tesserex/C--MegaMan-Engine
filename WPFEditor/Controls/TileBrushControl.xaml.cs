using System.Windows.Controls;
using MegaMan.Editor.Controls.ViewModels;
using Ninject;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for TileBrushControl.xaml
    /// </summary>
    public partial class TileBrushControl : UserControl
    {
        public TileBrushControl()
        {
            InitializeComponent();
            this.DataContext = App.Container.Get<TileBrushControlViewModel>();
        }
    }
}
