using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Bll.Tools
{
    public class EntityEditToolBehavior : IToolBehavior
    {
        private EntityPlacement _hoveredEntity;

        public void Click(ScreenCanvas canvas, Point location)
        {
            
        }

        public void Move(ScreenCanvas canvas, Point location)
        {
            _hoveredEntity = FindClosestEntity(canvas, location);
        }

        private EntityPlacement FindClosestEntity(ScreenCanvas canvas, Point location)
        {
            var project = canvas.Screen.Stage.Project;

            var entitiesWithInfo = canvas.Screen.Entities.Select(e => new {
                Placement = e,
                Info = project.EntityByName(e.entity)
            }).ToList();

            var entityBounds = entitiesWithInfo.Select(e => new {
                Placement = e.Placement,
                Bounds = new RectangleF(
                    e.Placement.screenX - e.Info.DefaultSprite.HotSpot.X,
                    e.Placement.screenY - e.Info.DefaultSprite.HotSpot.Y,
                    e.Info.DefaultSprite.Width,
                    e.Info.DefaultSprite.Height
                )
            }).ToList();

            var hoveredEntities = entityBounds
                .Where(e => e.Bounds.Contains(location))
                .ToList();

            if (hoveredEntities.Count == 1)
                return hoveredEntities.Single().Placement;
            else if (hoveredEntities.Count > 1)
                return hoveredEntities.OrderBy(e => DistanceSquaredFromCenter(e.Bounds, location)).First().Placement;
            else
                return null;
        }

        private float DistanceSquaredFromCenter(RectangleF bounds, Point location)
        {
            var center = new PointF((bounds.X + bounds.Width) / 2, (bounds.Y + bounds.Height) / 2);

            var dx = location.X - center.X;
            var dy = location.Y - center.Y;

            return (dx * dx) + (dy * dy);
        }

        public void Release(ScreenCanvas canvas, Point location)
        {
            
        }

        public void RightClick(ScreenCanvas canvas, Point location)
        {
            
        }
    }
}
