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
        }

        protected override void UnbindScreen(ScreenDocument oldScreen)
        {
            oldScreen.EntityAdded -= EntityAdded;
            oldScreen.EntityMoved -= EntityMoved;
            oldScreen.EntityRemoved -= EntityRemoved;

            foreach (var ctrl in this.Children.OfType<EntityPlacementControl>())
            {
                var vm = (EntityPlacementControlViewModel)ctrl.DataContext;
                vm.Destroy();
            }

            this.Children.Clear();
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
            var info = this.Screen.Stage.Project.EntityByName(placement.entity);
            var vm = new EntityPlacementControlViewModel(placement, info, Screen);
            ctrl.DataContext = vm;
            ctrl.Visibility = Visibility.Visible;

            PositionControl(ctrl);
            Canvas.SetZIndex(ctrl, 10000);

            vm.PlacementModified += (s, e) => PositionControl(ctrl);

            this.Children.Add(ctrl);
            Update();
        }

        private void EntityMoved(EntityPlacement placement)
        {
            var ctrl = this.Children.OfType<EntityPlacementControl>().SingleOrDefault(c => ((EntityPlacementControlViewModel)c.DataContext).Placement == placement);

            if (ctrl != null)
            {
                PositionControl(ctrl);
                Update();
            }
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
                bool flipHorizontal = (viewModel.Placement.direction == Direction.Left) ^ viewModel.DefaultSprite.Reversed;
                var offset = flipHorizontal ? viewModel.DefaultSprite.Width - viewModel.DefaultSprite.HotSpot.X : viewModel.DefaultSprite.HotSpot.X;

                Canvas.SetLeft(ctrl, Zoom * (viewModel.Placement.screenX - offset));
                Canvas.SetTop(ctrl, Zoom * (viewModel.Placement.screenY - viewModel.DefaultSprite.HotSpot.Y));
            }
            else
            {
                Canvas.SetLeft(ctrl, Zoom * viewModel.Placement.screenX);
                Canvas.SetTop(ctrl, Zoom * viewModel.Placement.screenY);
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
