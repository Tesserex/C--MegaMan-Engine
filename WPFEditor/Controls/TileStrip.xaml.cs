using MegaMan.Common;
using MegaMan.Editor.Controls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for TileStrip.xaml
    /// </summary>
    public partial class TileStrip : UserControl
    {
        public static readonly RoutedUICommand clickCommand = new RoutedUICommand("TileClick", "TileClick", typeof(TileStrip));

        public TileStrip()
        {
            InitializeComponent();
        }

        public void Update(TilesetViewModelBase viewModel)
        {
            DataContext = viewModel;
        }

        private void TileClick(object sender, ExecutedRoutedEventArgs e)
        {
            var tile = (Tile)e.Parameter;
            (DataContext as TilesetViewModelBase).ChangeTile(tile);
        }
    }
}
