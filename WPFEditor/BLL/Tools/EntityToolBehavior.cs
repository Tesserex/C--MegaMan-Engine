using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Bll.Tools
{
    public class EntityToolBehavior : IToolBehavior
    {
        private EntityInfo _entity;
        private int _snapX;
        private int _snapY;

        public EntityToolBehavior(EntityInfo entity, int snapX, int snapY)
        {
            _entity = entity;
            _snapX = snapX;
            _snapY = snapY;
        }

        public void Click(ScreenCanvas canvas, Point location)
        {
            var snappedPoint = new Point(
                (location.X / _snapX) * _snapX,
                (location.Y / _snapY) * _snapY);

            var placement = new Common.EntityPlacement() {
                entity = _entity.Name,
                direction = Common.Direction.Unknown,
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
    }
}
