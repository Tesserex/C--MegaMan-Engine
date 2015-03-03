using System.Windows.Controls;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for StagePropertiesControl.xaml
    /// </summary>
    public partial class StagePropertiesControl : UserControl
    {
        public StagePropertiesControl()
        {
            InitializeComponent();
            this.DataContext = new StagePropertiesViewModel();
        }
    }
}
