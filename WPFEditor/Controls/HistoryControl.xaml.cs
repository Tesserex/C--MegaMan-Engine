using System.Windows.Controls;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for HistoryControl.xaml
    /// </summary>
    public partial class HistoryControl : UserControl
    {
        public HistoryControl()
        {
            InitializeComponent();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                this.DataContext = new HistoryControlViewModel();
        }

        private void ChangeHistorySelection(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((HistoryControlViewModel)this.DataContext).MoveHistory(listView.SelectedIndex);
        }
    }
}
