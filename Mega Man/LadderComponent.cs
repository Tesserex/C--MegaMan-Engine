using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;

namespace Mega_Man
{
    public class LadderComponent : Component
    {
        private PositionComponent position;
        private MovementComponent movement;

        private List<HitBox> hitboxes = new List<HitBox>();

        private bool inReach = false;
        private bool inReachCached = false;
        private MapSquare inReachTile, aboveTile;
        private RectangleF reachRect;

        public bool InReach
        {
            get
            {
                if (!inReachCached) CheckInReach();
                return inReach;
            }
        }

        private bool atTop = false;
        private bool atTopCached = false;

        public bool AtTop
        {
            get
            {
                if (!atTopCached) CheckAtTop();
                return atTop;
            }
        }

        private bool aboveLadder = false;
        private bool aboveLadderCached = false;

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
            LadderComponent copy = new LadderComponent();
            copy.hitboxes = this.hitboxes;
            return copy;
        }

        public override void Start()
        {
            Engine.Instance.GameThink += Update;
        }

        public override void Stop()
        {
            Engine.Instance.GameThink -= Update;
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
            if (component is PositionComponent) this.position = component as PositionComponent;
            else if (component is MovementComponent) this.movement = component as MovementComponent;
        }

        public override void LoadXml(XElement xml)
        {
            foreach (XElement boxnode in xml.Elements("Hitbox"))
            {
                hitboxes.Add(new HitBox(boxnode));
            }
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
                position.SetPosition(new PointF(aboveTile.ScreenX + aboveTile.Tile.Width / 2, (Parent.GravityFlip && Game.CurrentGame.GravityFlip)? aboveTile.ScreenY + aboveTile.BoundBox.Height + 9 : aboveTile.ScreenY - 9));
            }
        }

        public void ClimbDown()
        {
            if (position != null && aboveTile != null)
            {
                position.SetPosition(new PointF(aboveTile.ScreenX + aboveTile.Tile.Width / 2, (Parent.GravityFlip && Game.CurrentGame.GravityFlip) ? aboveTile.ScreenY + aboveTile.BoundBox.Height + 2 : aboveTile.ScreenY - 2));
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

            foreach (HitBox hitbox in this.hitboxes)
            {
                foreach (MapSquare tile in Game.CurrentGame.CurrentMap.CurrentScreen.Tiles)
                {
                    if (tile.Tile.Properties.Climbable)
                    {
                        RectangleF myBox = hitbox.BoxAt(position.Position, Parent.GravityFlip ? Game.CurrentGame.GravityFlip : false);
                        RectangleF intersection = RectangleF.Intersect(tile.BoundBox, myBox);
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

            MegaMan.Tile above;
            if (Parent.GravityFlip && Game.CurrentGame.GravityFlip)
            {
                above = Game.CurrentGame.CurrentMap.CurrentScreen.Screen.TileAt(inReachTile.X, inReachTile.Y + 1);
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
                above = Game.CurrentGame.CurrentMap.CurrentScreen.Screen.TileAt(inReachTile.X, inReachTile.Y - 1);
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

            int tileX = (int)(position.Position.X / Game.CurrentGame.CurrentMap.Map.Tileset.TileSize);
            int tileY = (int)(position.Position.Y / Game.CurrentGame.CurrentMap.Map.Tileset.TileSize);

            MapSquare below;
            if (Parent.GravityFlip && Game.CurrentGame.GravityFlip)
            {
                below = Game.CurrentGame.CurrentMap.CurrentScreen.SquareAt(tileX, tileY - 1);
            }
            else below = Game.CurrentGame.CurrentMap.CurrentScreen.SquareAt(tileX, tileY + 1);

            aboveLadder = (below != null && below.Tile.Properties.Climbable);
            if (aboveLadder) aboveTile = below;
            aboveLadderCached = true;
        }

        public override Effect ParseEffect(XElement node)
        {
            Effect effect = (e) => { };

            if (node.Value == "Grab") effect = (entity) =>
            {
                LadderComponent ladder = entity.GetComponent<LadderComponent>();
                if (ladder != null) ladder.Grab();
            };
            else if (node.Value == "LetGo") effect = (entity) =>
            {
                LadderComponent ladder = entity.GetComponent<LadderComponent>();
                if (ladder != null) ladder.LetGo();
            };
            else if (node.Value == "StandOn") effect = (entity) =>
            {
                LadderComponent ladder = entity.GetComponent<LadderComponent>();
                if (ladder != null) ladder.StandOn();
            };
            else if (node.Value == "ClimbDown") effect = (entity) =>
            {
                LadderComponent ladder = entity.GetComponent<LadderComponent>();
                if (ladder != null) ladder.ClimbDown();
            };

            return effect;
        }
    }
}
