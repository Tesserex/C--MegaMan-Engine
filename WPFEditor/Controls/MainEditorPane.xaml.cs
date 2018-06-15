using System.Windows.Controls;
using MegaMan.Editor.Controls.ViewModels;
using Ninject;

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

            var tilesToolbarModel = new TilesToolbarViewModel();
            tileStrip.DataContext = tilesToolbarModel;

            var brushViewModel = App.Container.Get<TilePanelControlViewModel>();
            brushTray.DataContext = brushViewModel;

            stageTileControl.ToolProvider = tilesToolbarModel;

            var layoutEditor = new LayoutEditingViewModel();
            layoutToolbar.DataContext = layoutEditor;
            stageLayoutControl.ToolProvider = layoutEditor;

            var entityModel = new EntityTrayViewModel();
            stageEntitiesControl.ToolProvider = entityModel;
            entityTray.DataContext = entityModel;
            entityToolbar.DataContext = entityModel;
        }
    }
}
