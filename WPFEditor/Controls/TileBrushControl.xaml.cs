using System.Windows.Controls;
using System.Windows.Input;
using MegaMan.Editor.Bll.Tools;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for TileBrushControl.xaml
    /// </summary>
    public partial class TileBrushControl : UserControl
    {
        public static readonly RoutedUICommand clickCommand = new RoutedUICommand("BrushClick", "BrushClick", typeof(TileBrushControl));

        public TileBrushControl()
        {
            InitializeComponent();
        }

        private void BrushClick(object sender, ExecutedRoutedEventArgs e)
        {
            ((TileBrushControlViewModel)this.DataContext).SelectBrush((MultiTileBrush)e.Parameter);
        }
    }
}
