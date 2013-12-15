using MegaMan.Editor.Controls.ViewModels;
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

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for SpriteEditor.xaml
    /// </summary>
    public partial class SpriteEditor : UserControl
    {
        private SpriteEditorViewModel _viewModel;

        public SpriteEditor(Common.Sprite sprite)
        {
            _viewModel = new SpriteEditorViewModel();

            InitializeComponent();

            this.DataContext = _viewModel;
        }
    }
}
