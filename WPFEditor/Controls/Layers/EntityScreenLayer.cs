using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls {
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
            ctrl.Width = info.DefaultSprite.Width;
            ctrl.Height = info.DefaultSprite.Height;
            ctrl.Visibility = Visibility.Visible;

            Canvas.SetLeft(ctrl, placement.screenX - info.DefaultSprite.HotSpot.X);
            Canvas.SetTop(ctrl, placement.screenY - info.DefaultSprite.HotSpot.Y);

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
    }
}
