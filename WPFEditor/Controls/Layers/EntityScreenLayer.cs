using System.Linq;
using System.Windows;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    public class EntityScreenLayer : ScreenLayer
    {
        protected override void UnbindScreen(ScreenDocument oldScreen)
        {
            oldScreen.EntityAdded -= EntityAdded;
            oldScreen.EntityMoved -= EntityMoved;
            oldScreen.EntityRemoved -= EntityRemoved;

            foreach (var ctrl in Children.OfType<EntityPlacementControl>())
            {
                var vm = (EntityPlacementControlViewModel)ctrl.DataContext;
                vm.Destroy();
            }

            Children.Clear();
        }

        protected override void BindScreen(ScreenDocument newScreen)
        {
            newScreen.EntityAdded += EntityAdded;
            newScreen.EntityMoved += EntityMoved;
            newScreen.EntityRemoved += EntityRemoved;

            foreach (var placement in newScreen.Entities)
            {
                EntityAdded(placement);
            }
        }

        private void EntityAdded(EntityPlacement placement)
        {
            var ctrl = new EntityPlacementControl();
            var info = Screen.Stage.Project.EntityByName(placement.Entity);
            var vm = new EntityPlacementControlViewModel(placement, info, Screen);
            ctrl.DataContext = vm;
            ctrl.Visibility = Visibility.Visible;

            PositionControl(ctrl);
            SetZIndex(ctrl, 10000);

            vm.PlacementModified += (s, e) => PositionControl(ctrl);

            Children.Add(ctrl);
            Update();
        }

        private void EntityMoved(EntityPlacement placement)
        {
            var ctrl = Children.OfType<EntityPlacementControl>().SingleOrDefault(c => ((EntityPlacementControlViewModel)c.DataContext).Placement == placement);

            if (ctrl != null)
            {
                PositionControl(ctrl);
                Update();
            }
        }

        private void EntityRemoved(EntityPlacement placement)
        {
            var ctrl = Children.OfType<EntityPlacementControl>().SingleOrDefault(c => ((EntityPlacementControlViewModel)c.DataContext).Placement == placement);

            if (ctrl != null)
                Children.Remove(ctrl);

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
                PositionControl(c);
                c.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return base.MeasureOverride(constraint);
        }

        private void PositionControl(EntityPlacementControl ctrl)
        {
            var viewModel = (EntityPlacementControlViewModel)ctrl.DataContext;
            if (viewModel.DefaultSprite != null)
            {
                bool flipHorizontal = (viewModel.Placement.Direction == Direction.Left) ^ viewModel.DefaultSprite.Reversed;
                var offset = flipHorizontal ? viewModel.DefaultSprite.Width - viewModel.DefaultSprite.HotSpot.X : viewModel.DefaultSprite.HotSpot.X;

                SetLeft(ctrl, Zoom * (viewModel.Placement.ScreenX - offset));
                SetTop(ctrl, Zoom * (viewModel.Placement.ScreenY - viewModel.DefaultSprite.HotSpot.Y));
            }
            else
            {
                SetLeft(ctrl, Zoom * viewModel.Placement.ScreenX);
                SetTop(ctrl, Zoom * viewModel.Placement.ScreenY);
            }

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
    }
}
