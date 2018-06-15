using System.Windows.Controls;
using System.Windows.Documents;
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

            Loaded += (s, e) => {
                adornerLayer = AdornerLayer.GetAdornerLayer(hitboxRect);
                adornerLayer.Add(new HitboxResizeAdorner(hitboxRect, DataContext as HitboxEditorViewModel));
            };
        }
    }
}
