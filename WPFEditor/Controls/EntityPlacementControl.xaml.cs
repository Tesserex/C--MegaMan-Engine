using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for EntityPlacementControl.xaml
    /// </summary>
    public partial class EntityPlacementControl : UserControl
    {
        public Point DragOrigin { get; private set; }

        public EntityPlacementControl()
        {
            InitializeComponent();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ((EntityPlacementControlViewModel)DataContext).Hovered = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            ((EntityPlacementControlViewModel)DataContext).Hovered = false;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            DragOrigin = e.GetPosition(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
            }
        }
    }
}
