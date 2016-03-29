using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
        public static readonly DependencyProperty MultiSelectProperty = DependencyProperty.Register("MultiSelect", typeof(bool), typeof(TilePanel), new PropertyMetadata(false));
        public static readonly DependencyProperty SelectedTileProperty = DependencyProperty.Register("SelectedTile", typeof(Tile), typeof(TilePanel));
        public static readonly DependencyProperty SelectedTilesProperty = DependencyProperty.Register("SelectedTiles", typeof(IEnumerable<Tile>), typeof(TilePanel));

        public TilePanel()
        {
            InitializeComponent();

            this.tileList.SelectionChanged += TileList_SelectionChanged;
        }

        private void TileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTiles = tileList.SelectedItems.Cast<Tile>().ToList();

            if (tileList.SelectedItems.Count == 1)
                SelectedTile = (Tile)tileList.SelectedItem;
            else
                SelectedTile = null;
        }
        
        public bool MultiSelect
        {
            get { return (bool)GetValue(MultiSelectProperty); }
            set { SetValue(MultiSelectProperty, value); }
        }

        public Tile SelectedTile
        {
            get { return (Tile)GetValue(SelectedTileProperty); }
            set
            {
                SetValue(SelectedTileProperty, value);
                if (this.DataContext is TilesetViewModelBase)
                    ((TilesetViewModelBase)this.DataContext).SelectedTile = value;
            }
        }

        public IEnumerable<Tile> SelectedTiles
        {
            get { return (IEnumerable<Tile>)GetValue(SelectedTilesProperty); }
            set
            {
                SetValue(SelectedTilesProperty, value);
                if (this.DataContext is TilesetViewModelBase)
                    ((TilesetViewModelBase)this.DataContext).MultiSelectedTiles = value;
            }
        }

        public SelectionMode SelectionMode
        {
            get
            {
                if (MultiSelect)
                    return SelectionMode.Extended;
                else
                    return SelectionMode.Single;
            }
        }
    }
}
