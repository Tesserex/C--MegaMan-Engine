using System;
using System.Collections.Generic;
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
using MegaMan.Editor.Controls.Adorners;
using MegaMan.Editor.Controls.ViewModels.Entities.Components;

namespace MegaMan.Editor.Controls.Components
{
    /// <summary>
    /// Interaction logic for HitboxEditor.xaml
    /// </summary>
    public partial class HitboxEditor : UserControl
    {
        private AdornerLayer adornerLayer;

        public HitboxEditor()
        {
            InitializeComponent();

            this.Loaded += (s, e) => {
                adornerLayer = AdornerLayer.GetAdornerLayer(hitboxRect);
                adornerLayer.Add(new HitboxResizeAdorner(hitboxRect, this.DataContext as HitboxEditorViewModel));
            };
        }
    }
}
