using System.ComponentModel;
using System.Windows.Controls;
using MegaMan.Editor.Controls.ViewModels.Entities;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for EntityEditor.xaml
    /// </summary>
    public partial class EntityEditor : UserControl
    {
        public EntityEditor()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = new EntityEditorViewModel();
            }
        }
    }
}
