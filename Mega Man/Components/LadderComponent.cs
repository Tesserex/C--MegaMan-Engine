using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    public class LadderComponent : Component
    {
        private PositionComponent position;

        private List<HitBox> hitboxes = new List<HitBox>();

        private bool inReach;
        private bool inReachCached;
        private MapSquare inReachTile, aboveTile;
        private Rectangle reachRect;

        public bool InReach
        {
            get
            {
                if (!inReachCached) CheckInReach();
                return inReach;
            }
        }

        private bool atTop;
        private bool atTopCached;

        public bool AtTop
        {
            get
            {
                if (!atTopCached) CheckAtTop();
                return atTop;
            }
        }

        private bool aboveLadder;
        private bool aboveLadderCached;

        public bool AboveLadder
        {
            get
            {
                if (!aboveLadderCached) CheckAboveLadder();
                return aboveLadder;
            }
        }

        public override Component Clone()
        {
            LadderComponent copy = new LadderComponent {hitboxes = this.hitboxes};
            return copy;
        }

        public override void Start(IGameplayContainer container)
        {
            container.GameThink += Update;
        }

        public override void Stop(IGameplayContainer container)
        {
            container.GameThink -= Update;
        }

        public override void Message(IGameMessage msg)
        {
            
        }

        protected override void Update()
        {
            aboveLadderCached = false;
            atTopCached = false;
            inReachCached = false;
            inReachTile = null;
            aboveTile = null;
        }

        public override void RegisterDependencies(Component component)
        {
            if (component is PositionComponent) position = component as PositionComponent;
        }

        public void LoadInfo(LadderComponentInfo info)
        {
            hitboxes.AddRange(info.HitBoxes.Select(h => new HitBox(h.Box.X, h.Box.Y, h.Box.Width, h.Box.Height)));
        }

        public void Grab()
        {
            if (InReach)
            {
                position.SetPosition(new PointF(inReachTile.ScreenX + inReachTile.Tile.Width / 2, position.Position.Y));
            }
        }

        public void StandOn()
        {
            if (position != null && aboveTile != null)
            {
                position.SetPosition(new PointF(aboveTile.ScreenX + aboveTile.Tile.Width / 2, (Parent.IsGravitySensitive && Parent.Container.IsGravityFlipped) ? aboveTile.ScreenY + aboveTile.BoundBox.Height + 9 : aboveTile.ScreenY - 9));
            }
        }

        public void ClimbDown()
        {
            if (position != null && aboveTile != null)
            {
                position.SetPosition(new PointF(aboveTile.ScreenX + aboveTile.Tile.Width / 2, (Parent.IsGravitySensitive && Parent.Container.IsGravityFlipped) ? aboveTile.ScreenY + aboveTile.BoundBox.Height + 2 : aboveTile.ScreenY - 2));
            }
        }

        public void LetGo()
        {
            inReach = false;
        }

        private void CheckInReach()
        {
            inReach = false;
            if (position == null) return;

            foreach (HitBox hitbox in hitboxes)
            {
                foreach (MapSquare tile in Parent.Screen.Tiles)
                {
                    if (tile.Tile.Properties.Climbable)
                    {
                        var myBox = hitbox.BoxAt(position.Position, Parent.IsGravitySensitive ? Parent.Container.IsGravityFlipped : false);
                        var intersection = Rectangle.Intersect(tile.BoundBox, myBox);
                        if (!intersection.IsEmpty)
                        {
                            inReach = true;
                            inReachTile = tile;
                            inReachCached = true;
                            reachRect = intersection;
                            return;
                        }
                    }
                }
            }
        }

        private void CheckAtTop()
        {
            atTop = false;
            if (!InReach) return;

            Tile above;
            if (Parent.IsGravitySensitive && Parent.Container.IsGravityFlipped)
            {
                above = Parent.Screen.TileAt(inReachTile.X * Parent.Screen.TileSize, (inReachTile.Y + 1) * Parent.Screen.TileSize);
                if (above != null && !above.Properties.Climbable)
                {
                    if (reachRect.Height < 5 && reachRect.Bottom == inReachTile.BoundBox.Bottom)	// at the top
                    {
                        atTop = true;
                    }
                }
            }
            else
            {
                above = Parent.Screen.TileAt(inReachTile.X * Parent.Screen.TileSize, (inReachTile.Y - 1) * Parent.Screen.TileSize);
                if (above != null && !above.Properties.Climbable)
                {
                    if (reachRect.Height < 5 && reachRect.Top == inReachTile.BoundBox.Top)	// at the top
                    {
                        atTop = true;
                    }
                }
            }
            atTopCached = true;
        }

        private void CheckAboveLadder()
        {
            aboveLadder = false;
            if (position == null) return;

            var px = position.Position.X;
            var py = position.Position.Y;

            MapSquare below;
            if (Parent.IsGravitySensitive && Parent.Container.IsGravityFlipped)
            {
                below = Parent.Screen.SquareAt(px, py - Parent.Screen.TileSize);
            }
            else below = Parent.Screen.SquareAt(px, py + Parent.Screen.TileSize);

            aboveLadder = (below != null && below.Tile.Properties.Climbable);
            if (aboveLadder) aboveTile = below;
            aboveLadderCached = true;
        }
    }
}
