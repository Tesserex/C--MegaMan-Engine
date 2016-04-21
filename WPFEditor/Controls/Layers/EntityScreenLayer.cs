using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    public class EntityScreenLayer : ScreenLayer
    {
        public EntityScreenLayer()
        {
            this.AllowDrop = true;
        }

        protected override void UnbindScreen(ScreenDocument oldScreen)
        {
            oldScreen.EntityAdded -= EntityAdded;
            oldScreen.EntityRemoved -= EntityRemoved;

            this.Children.Clear();
        }

        protected override void BindScreen(ScreenDocument newScreen)
        {
            newScreen.EntityAdded += EntityAdded;
            newScreen.EntityRemoved += EntityRemoved;

            foreach (var placement in newScreen.Entities)
            {
                EntityAdded(placement);
            }
        }

        private void EntityAdded(EntityPlacement placement)
        {
            var ctrl = new EntityPlacementControl();
            var info = this.Screen.Stage.Project.EntityByName(placement.entity);
            var vm = new EntityPlacementControlViewModel(placement, info, Screen);
            ctrl.DataContext = vm;
            ctrl.Visibility = Visibility.Visible;

            PositionControl(ctrl, vm);
            Canvas.SetZIndex(ctrl, 10000);

            vm.PlacementModified += (s, e) => PositionControl(ctrl, vm);

            this.Children.Add(ctrl);
            Update();
        }

        private void EntityRemoved(EntityPlacement placement)
        {
            var ctrl = this.Children.OfType<EntityPlacementControl>().SingleOrDefault(c => ((EntityPlacementControlViewModel)c.DataContext).Placement == placement);

            if (ctrl != null)
                this.Children.Remove(ctrl);

            Update();
        }

        protected override void Update()
        {
            InvalidateVisual();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            foreach (var c in Children.OfType<EntityPlacementControl>())
            {
                var d = ((EntityPlacementControlViewModel)c.DataContext);
                PositionControl(c, d);

                c.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return base.MeasureOverride(constraint);
        }

        private void PositionControl(EntityPlacementControl ctrl, EntityPlacementControlViewModel viewModel)
        {
            bool flipHorizontal = (viewModel.Placement.direction == Direction.Left) ^ viewModel.DefaultSprite.Reversed;
            var offset = flipHorizontal ? viewModel.DefaultSprite.Width - viewModel.DefaultSprite.HotSpot.X : viewModel.DefaultSprite.HotSpot.X;

            Canvas.SetLeft(ctrl, Zoom * (viewModel.Placement.screenX - offset));
            Canvas.SetTop(ctrl, Zoom * (viewModel.Placement.screenY - viewModel.DefaultSprite.HotSpot.Y));
            InvalidateVisual();
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (var c in Children.OfType<EntityPlacementControl>())
            {
                c.Arrange(new Rect(GetLeft(c) * Zoom, GetTop(c) * Zoom, c.DesiredSize.Width * Zoom, c.DesiredSize.Height * Zoom));
            }

            return base.ArrangeOverride(arrangeSize);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            var ctrl = (EntityPlacementControl)e.Data.GetData(typeof(EntityPlacementControl));
            if (ctrl != null)
            {
                var vm = (EntityPlacementControlViewModel)ctrl.DataContext;
                var canvasPoint = e.GetPosition(this);
                var screenMouseX = canvasPoint.X / Zoom;
                var screenMouseY = canvasPoint.Y / Zoom;

                var offsetX = vm.DefaultSprite.HotSpot.X - (ctrl.DragOrigin.X / Zoom);
                var offsetY = vm.DefaultSprite.HotSpot.Y - (ctrl.DragOrigin.Y / Zoom);

                vm.Placement.screenX = (float)(screenMouseX + offsetX);
                vm.Placement.screenY = (float)(screenMouseY + offsetY);

                Screen.Stage.Dirty = true;

                PositionControl(ctrl, vm);
            }
        }
    }
}
