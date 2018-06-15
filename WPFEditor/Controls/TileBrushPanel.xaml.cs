using System.Windows.Controls;
using System.Windows.Input;
using MegaMan.Editor.Bll.Tools;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for TilePanelControl.xaml
    /// </summary>
    public partial class TileBrushPanel : UserControl
    {
        public static readonly RoutedUICommand clickBrushCommand = new RoutedUICommand("BrushClick", "BrushClick", typeof(TileBrushPanel));

        public TileBrushPanel()
        {
            InitializeComponent();
        }

        private void BrushClick(object sender, ExecutedRoutedEventArgs e)
        {
            ((TilePanelControlViewModel)DataContext).SelectBrush((MultiTileBrush)e.Parameter);
        }
    }
}
