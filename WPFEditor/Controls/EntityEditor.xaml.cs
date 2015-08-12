using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
