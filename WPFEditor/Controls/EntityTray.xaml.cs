using System.Windows.Controls;
using System.Windows.Input;
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
        
        private void MouseLeftDown(object sender, MouseButtonEventArgs e)
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
