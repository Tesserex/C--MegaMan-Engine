using System.Windows.Controls;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for TilesetEditor.xaml
    /// </summary>
    public partial class TilesetEditor : UserControl
    {
        public TilesetEditor()
        {
            InitializeComponent();
            DataContext = new TilesetEditorViewModel();
        }
    }
}
