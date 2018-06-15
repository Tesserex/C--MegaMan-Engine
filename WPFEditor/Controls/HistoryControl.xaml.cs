using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
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

            if (!DesignerProperties.GetIsInDesignMode(this))
                DataContext = new HistoryControlViewModel();
        }

        private void ChangeHistorySelection(object sender, MouseButtonEventArgs e)
        {
            ((HistoryControlViewModel)DataContext).MoveHistory(listView.SelectedIndex);
        }
    }
}
