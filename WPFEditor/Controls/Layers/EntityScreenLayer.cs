using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            ctrl.DataContext = new EntityPlacementControlViewModel(placement, info);
            ctrl.Visibility = Visibility.Visible;

            Canvas.SetLeft(ctrl, placement.screenX - info.DefaultSprite.HotSpot.X);
            Canvas.SetTop(ctrl, placement.screenY - info.DefaultSprite.HotSpot.Y);
            Canvas.SetZIndex(ctrl, 10000);

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
                Canvas.SetLeft(c, Zoom * (d.Placement.screenX - d.DefaultSprite.HotSpot.X));
                Canvas.SetTop(c, Zoom * (d.Placement.screenY - d.DefaultSprite.HotSpot.Y));

                c.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return base.MeasureOverride(constraint);
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
