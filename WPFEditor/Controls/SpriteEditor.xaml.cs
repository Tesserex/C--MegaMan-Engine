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
        public SpriteEditor()
        {
            InitializeComponent();
        }

        public SpriteEditor(Common.Sprite sprite)
        {
            var viewModel = new SpriteEditorViewModel(sprite);

            InitializeComponent();

            this.DataContext = viewModel;
        }

        private void SheetMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(sheetImage);

            if (snapSheet.IsChecked == true)
            {
                pos = new Point(Math.Floor(pos.X / sheetHighlight.Width) * sheetHighlight.Width, Math.Floor(pos.Y / sheetHighlight.Height) * sheetHighlight.Height);
            }

            Canvas.SetTop(sheetHighlight, pos.Y);
            Canvas.SetLeft(sheetHighlight, pos.X);
        }

        private void SheetMouseEnter(object sender, MouseEventArgs e)
        {
            var viewModel = DataContext as SpriteEditorViewModel;
            if (viewModel.Sprite.Playing == false)
                sheetHighlight.Visibility = Visibility.Visible;
        }

        private void SheetMouseLeave(object sender, MouseEventArgs e)
        {
            sheetHighlight.Visibility = Visibility.Hidden;
        }

        private void SheetMouseClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = DataContext as SpriteEditorViewModel;
            var x = (int)Canvas.GetLeft(sheetHighlight);
            var y = (int)Canvas.GetTop(sheetHighlight);
            viewModel.SetFrameLocation(x, y);
        }
    }
}
