using System.Windows.Controls;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Editor.Bll.Tools;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for TilePanelControl.xaml
    /// </summary>
    public partial class TilePanelControl : UserControl
    {
        public static readonly RoutedUICommand clickBrushCommand = new RoutedUICommand("BrushClick", "BrushClick", typeof(TilePanelControl));

        public static readonly RoutedUICommand clickTileCommand = new RoutedUICommand("TileClick", "TileClick", typeof(TilePanelControl));

        public TilePanelControl()
        {
            InitializeComponent();
        }

        private void BrushClick(object sender, ExecutedRoutedEventArgs e)
        {
            ((TilePanelControlViewModel)this.DataContext).SelectBrush((MultiTileBrush)e.Parameter);
        }

        private void TileClick(object sender, ExecutedRoutedEventArgs e)
        {
            ((TilePanelControlViewModel)this.DataContext).ChangeTile((Tile)e.Parameter);
        }
    }
}
