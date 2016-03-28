using System.Windows.Controls;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for TilePanel.xaml
    /// </summary>
    public partial class TilePanel : UserControl
    {
        public TilePanel()
        {
            InitializeComponent();
        }

        public static readonly RoutedUICommand clickTileCommand = new RoutedUICommand("TileClick", "TileClick", typeof(TilePanel));

        private void TileClick(object sender, ExecutedRoutedEventArgs e)
        {
            ((TilesetViewModelBase)this.DataContext).ChangeTile((Tile)e.Parameter);
        }
    }
}
