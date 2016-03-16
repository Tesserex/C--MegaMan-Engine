using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for SpriteEditor.xaml
    /// </summary>
    public partial class SpriteEditor : UserControl
    {
        public static readonly DependencyProperty CanChangeSizeProperty = DependencyProperty.Register("CanChangeSize", typeof(bool), typeof(SpriteEditor), new PropertyMetadata(true));

        public bool CanChangeSize
        {
            get { return (bool)GetValue(CanChangeSizeProperty); }
            set { SetValue(CanChangeSizeProperty, value); }
        }

        public Visibility SizeChangeVisibility
        {
            get
            {
                return CanChangeSize ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public SpriteEditor()
        {
            InitializeComponent();
        }

        public SpriteEditor(Common.Sprite sprite)
        {
            var viewModel = new SpriteEditorViewModel(new SpriteViewModel(sprite));

            InitializeComponent();

            this.DataContext = viewModel;
        }

        private void SheetMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(sheetImage);

            if (snapSheet.IsChecked == true)
            {
                var viewModel = DataContext as SpriteEditorViewModel;
                var spacing = sheetHighlight.Width + (snapGap.Value.Value * viewModel.SheetZoom);
                pos = new Point(Math.Floor(pos.X / spacing) * spacing, Math.Floor(pos.Y / spacing) * spacing);
            }

            Canvas.SetTop(sheetHighlight, pos.Y);
            Canvas.SetLeft(sheetHighlight, pos.X);
        }

        private void SheetMouseEnter(object sender, MouseEventArgs e)
        {
            if (DataContext == null)
                return;

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
            if (DataContext == null)
                return;

            var viewModel = DataContext as SpriteEditorViewModel;
            var x = (int)Canvas.GetLeft(sheetHighlight);
            var y = (int)Canvas.GetTop(sheetHighlight);
            viewModel.SetFrameLocation(x, y);
        }
    }
}
