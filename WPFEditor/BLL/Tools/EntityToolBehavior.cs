using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Bll.Tools
{
    public class EntityToolBehavior : IEntityToolBehavior
    {
        private EntityInfo _entity;
        
        public int SnapX { get; set; }
        public int SnapY { get; set; }

        public EntityToolBehavior(EntityInfo entity, int snapX, int snapY)
        {
            _entity = entity;
            SnapX = snapX;
            SnapY = snapY;
        }

        public void Click(ScreenCanvas canvas, Point location)
        {
            var snappedPoint = new Point(
                (location.X / SnapX) * SnapX,
                (location.Y / SnapY) * SnapY);

            var placement = new EntityPlacement {
                entity = _entity.Name,
                direction = Direction.Left,
                screenX = snappedPoint.X,
                screenY = snappedPoint.Y
            };

            canvas.Screen.AddEntity(placement);

            canvas.Screen.Stage.PushHistoryAction(new AddEntityAction(placement, canvas.Screen));
        }

        public void Move(ScreenCanvas canvas, Point location)
        {

        }

        public void Release(ScreenCanvas canvas, Point location)
        {

        }

        public void RightClick(ScreenCanvas canvas, Point location)
        {
            
        }

        public bool SuppressContextMenu { get { return false; } }
    }
}
