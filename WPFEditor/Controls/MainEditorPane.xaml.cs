using System.Windows.Controls;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for MainEditorPane.xaml
    /// </summary>
    public partial class MainEditorPane : UserControl
    {
        public MainEditorPane()
        {
            InitializeComponent();

            var tilesetModel = new TilesetViewModel();
            tileStrip.DataContext = tilesetModel;
            stageTileControl.ToolProvider = tilesetModel;

            var layoutEditor = new LayoutEditingViewModel();
            layoutToolbar.DataContext = layoutEditor;
            stageLayoutControl.ToolProvider = layoutEditor;
        }
    }
}
