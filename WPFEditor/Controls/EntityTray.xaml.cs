using System.Windows.Controls;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for EntityTray.xaml
    /// </summary>
    public partial class EntityTray : UserControl
    {
        public EntityTray()
        {
            InitializeComponent();
        }
        
        private void MouseLeftDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                var vm = (EntityTrayViewModel)DataContext;
                vm.UpdateTool("Entity");
            }
        }
    }
}
