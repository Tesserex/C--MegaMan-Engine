using System.Windows.Controls;
using MegaMan.Editor.Controls.ViewModels;
using MegaMan.Editor.Tools;
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

            var tilesetModel = new TilesetViewModel();
            tileStrip.DataContext = tilesetModel;

            var brushViewModel = App.Container.Get<TileBrushControlViewModel>();
            brushTray.DataContext = brushViewModel;

            var tilingToolProvider = new CombinedToolProvider(tilesetModel, brushViewModel);
            stageTileControl.ToolProvider = tilingToolProvider;

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
