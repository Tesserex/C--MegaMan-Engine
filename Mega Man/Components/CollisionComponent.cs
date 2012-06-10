using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using MegaMan.Common;

namespace MegaMan.Engine
{
    [System.Diagnostics.DebuggerDisplay("Parent = {Parent.Name}, BlockTop: {BlockTop}, BlockLeft: {BlockLeft}, BlockRight: {BlockRight}, BlockBottom: {BlockBottom}")]
    public class CollisionComponent : Component
    {
        private Texture2D rectTex;

        private class Collision
        {
            public readonly CollisionBox myBox;
            public readonly CollisionComponent targetColl;
            public readonly CollisionBox targetBox;

            public Collision(CollisionBox mybox, CollisionBox target, CollisionComponent tgtComp)
            {
                myBox = mybox;
                targetBox = target;
                targetColl = tgtComp;
            }
        }

        private List<CollisionBox> hitboxes = new List<CollisionBox>();
        private readonly HashSet<string> touchedBy = new HashSet<string>();
        private readonly Dictionary<string, HashSet<string>> touchedAt = new Dictionary<string, HashSet<string>>();
        private readonly HashSet<int> enabledBoxes = new HashSet<int>();
        private readonly Dictionary<string, int> boxIDsByName = new Dictionary<string, int>();

        public float DamageDealt { get; private set; }

        public bool Enabled { get; set; }
        public bool BlockTop { get; private set; }
        public bool BlockLeft { get; private set; }
        public bool BlockRight { get; private set; }
        public bool BlockBottom { get; private set; }
        public bool Touched { get { return touchedBy.Count > 0; } }

        private float blockBottomMin, blockBottomMax;
        public float BlockBottomWidth { get { return blockBottomMax - blockBottomMin; } }

        private float blockTopMin, blockTopMax;
        public float BlockTopWidth { get { return blockTopMax - blockTopMin; } }

        private float blockLeftMin, blockLeftMax;
        public float BlockLeftWidth { get { return blockLeftMax - blockLeftMin; } }

        private float blockRightMin, blockRightMax;
        public float BlockRightWidth { get { return blockRightMax - blockRightMin; } }

        public PositionComponent PositionSrc { get; private set; }
        public MovementComponent MovementSrc { get; private set; }

        public override Component Clone()
        {
            CollisionComponent copy = new CollisionComponent
            {
                Enabled = Enabled,
                hitboxes = new List<CollisionBox>()
            };
            foreach (CollisionBox box in hitboxes) copy.AddBox(box);

            return copy;
        }

        public override void Start()
        {
            Enabled = true;
            Parent.Container.GameAct += ClearTouch;
            Parent.Container.GameReact += Update;
            Parent.Container.Draw += Instance_GameRender;

            rectTex = new Texture2D(Engine.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            rectTex.SetData(new[] { new Microsoft.Xna.Framework.Color(1, 0.6f, 0, 0.7f) });
        }

        private void ClearTouch()
        {
            touchedBy.Clear();
            touchedAt.Clear();
        }

        void Instance_GameRender(GameRenderEventArgs e)
        {
            if (!Engine.Instance.DrawHitboxes) return;
            foreach (CollisionBox hitbox in hitboxes)
            {
                if (!enabledBoxes.Contains(hitbox.ID)) continue;

                RectangleF boundBox = hitbox.BoxAt(PositionSrc.Position);
                boundBox.Offset(-Parent.Screen.OffsetX, -Parent.Screen.OffsetY);
                if (Engine.Instance.Foreground) e.Layers.ForegroundBatch.Draw(rectTex, new Microsoft.Xna.Framework.Rectangle((int)(boundBox.X), (int)(boundBox.Y), (int)(boundBox.Width), (int)(boundBox.Height)), Microsoft.Xna.Framework.Color.White);
            }
        }

        public override void Stop()
        {
            Parent.Container.GameAct -= ClearTouch;
            Parent.Container.GameReact -= Update;
            Parent.Container.Draw -= Instance_GameRender;
            Enabled = false;
            ClearTouch();
            enabledBoxes.Clear();
        }

        public override void Message(IGameMessage msg)
        {
            HitBoxMessage boxes = msg as HitBoxMessage;
            if (boxes != null)
            {
                foreach (CollisionBox box in boxes.AddBoxes)
                {
                    box.SetParent(this);
                    AddBox(box);
                }

                enabledBoxes.Clear();
                foreach (var name in boxes.EnableBoxes)
                {
                    if (!boxIDsByName.ContainsKey(name))
                    {
                        throw new GameRunException(String.Format("The {0} entity wanted to enable a hitbox named {1}, which doesn't exist.", Parent.Name, name));
                    }

                    enabledBoxes.Add(boxIDsByName[name]);
                }

                return;
            }
        }

        public override void LoadXml(XElement xml)
        {
            foreach (XElement boxnode in xml.Elements("Hitbox"))
            {
                CollisionBox box = new CollisionBox(boxnode);
                box.SetParent(this);
                AddBox(box);
            }
            bool b;
            if (xml.TryBool("Enabled", out b))
            {
                Enabled = b;
            }
        }

        private void AddBox(CollisionBox box)
        {
            hitboxes.Add(box);
            if (box.Name != null) boxIDsByName.Add(box.Name, box.ID);
        }

        protected override void Update()
        {
            DamageDealt = 0;
            BlockTop = BlockRight = BlockLeft = BlockBottom = false;
            blockBottomMin = blockLeftMin = blockRightMin = blockTopMin = float.PositiveInfinity;
            blockBottomMax = blockTopMax = blockLeftMax = blockRightMax = float.NegativeInfinity;
            if (PositionSrc == null) return;
            if (!Enabled) return;

            // first run through, resolve intersections only
            List<MapSquare> hitSquares = new List<MapSquare>();
            List<Collision> blockEntities = new List<Collision>();
            foreach (CollisionBox hitbox in hitboxes)
            {
                if (!enabledBoxes.Contains(hitbox.ID)) continue;

                hitbox.SetParent(this);
                if (hitbox.Environment) // check collision with environment
                {
                    CheckEnvironment(hitSquares, hitbox);
                }

                RectangleF boundbox = hitbox.BoxAt(PositionSrc.Position);

                // now check with entity blocks
                if (MovementSrc != null)
                {
                    boundbox = CheckEntityCollisions(blockEntities, hitbox, boundbox);
                }
            }

            // this stores the types of tile we're touching
            HashSet<TileProperties> hitTypes = new HashSet<TileProperties>();

            // first, as an aside, if i'm still touching a blocking entity, i need to react to that
            foreach (Collision collision in blockEntities)
            {
                RectangleF boundBox = collision.myBox.BoxAt(PositionSrc.Position);
                RectangleF rect = collision.targetBox.BoxAt(collision.targetColl.PositionSrc.Position);
                if (BlockByIntersection(boundBox, rect, false, false))
                {
                    collision.targetColl.Touch(collision.myBox, collision.targetBox);
                    Touch(collision.targetBox, collision.myBox);
                    // for now, entities can only be normal type
                    hitTypes.Add(collision.targetBox.Properties);
                    // now cause friction on the x, a la moving platforms
                    if (collision.targetColl.MovementSrc != null && collision.myBox.PushAway)
                    {
                        PositionSrc.Offset(collision.targetColl.MovementSrc.VelocityX, 0);
                    }
                }
            }

            // now determine who i'm still touching, the resulting effects
            // notice - there's really no one to speak on behalf of the environment,
            // so we need to inflict the effects upon ourself
            foreach (CollisionBox hitbox in hitboxes)
            {
                if (!enabledBoxes.Contains(hitbox.ID)) continue;
                ReactForHitbox(hitSquares, hitTypes, hitbox);
            }

            if (MovementSrc != null)
            {
                if (BlockTop && MovementSrc.VelocityY < 0) MovementSrc.VelocityY = 0;
                if (BlockLeft && MovementSrc.VelocityX < 0) MovementSrc.VelocityX = 0;
                if (BlockRight && MovementSrc.VelocityX > 0) MovementSrc.VelocityX = 0;
                if (BlockBottom && MovementSrc.VelocityY > 0) MovementSrc.VelocityY = 0;
            }

            // react to tile effects
            foreach (TileProperties tileprops in hitTypes)
            {
                ReactToTileEffect(tileprops);
            }
        }

        private void ReactForHitbox(List<MapSquare> hitSquares, HashSet<TileProperties> hitTypes, CollisionBox hitbox)
        {
            hitbox.SetParent(this);

            RectangleF boundBox = hitbox.BoxAt(PositionSrc.Position);

            if (hitbox.Environment)
            {
                foreach (MapSquare tile in hitSquares)
                {
                    RectangleF tileBox = tile.BlockBox;
                    bool downonly = (!Game.CurrentGame.GravityFlip && tile.Tile.Properties.Climbable);
                    bool uponly = (Game.CurrentGame.GravityFlip && tile.Tile.Properties.Climbable);

                    bool hit = (tile.BlockBox != RectangleF.Empty) ? BlockByIntersection(boundBox, tileBox, uponly, downonly) : boundBox.IntersectsWith(tile.BoundBox);

                    if (hitbox.PushAway && (hit || boundBox.IntersectsWith(tile.BoundBox)))    // the environment touched me!
                    {
                        hitTypes.Add(tile.Tile.Properties);
                    }
                }
            }

            // but for entities, we can go ahead and be active aggressors -
            // inflict our effects on the target entity, not the other way around
            foreach (GameEntity entity in GameEntity.GetAll())
            {
                if (entity == Parent) continue;
                CollisionComponent coll = entity.GetComponent<CollisionComponent>();
                if (coll == null) continue;

                foreach (CollisionBox targetBox in coll.TargetBoxes(hitbox))
                {
                    boundBox = CheckTargetBox(hitbox, boundBox, entity, coll, targetBox);
                }
            }
        }

        private RectangleF CheckTargetBox(CollisionBox hitbox, RectangleF boundBox, GameEntity entity, CollisionComponent coll, CollisionBox targetBox)
        {
            RectangleF rect = targetBox.BoxAt(coll.PositionSrc.Position);
            if (boundBox.IntersectsWith(rect))
            {
                coll.Touch(hitbox, targetBox);
                Touch(targetBox, hitbox);
                CollideWith(entity, hitbox, targetBox);
            }
            return boundBox;
        }

        private void CheckEnvironment(List<MapSquare> hitSquares, CollisionBox hitbox)
        {
            PointF offset = new PointF(0, 0);
            RectangleF hitRect = hitbox.BoxAt(PositionSrc.Position);

            // this bounds checking prevents needlessly checking collisions way too far away
            // it's a very effective optimization (brings busy time from ~60% down to 45%!)
            int size = Parent.Screen.TileSize;
            int minx = (int)(hitRect.Left / size) - 1;
            int miny = (int)(hitRect.Top / size) - 1;
            int maxx = (int)(hitRect.Right / size) + 1;
            int maxy = (int)(hitRect.Bottom / size) + 1;
            for (int y = miny; y <= maxy; y++)
                for (int x = minx; x <= maxx; x++)
                {
                    MapSquare tile = Parent.Screen.SquareAt(x, y);
                    if (tile == null) continue;
                    if (hitbox.EnvironmentCollisions(PositionSrc.Position, tile, ref offset))
                    {
                        hitSquares.Add(tile);
                        if (hitbox.PushAway) PositionSrc.Offset(offset.X, offset.Y);
                    }
                    else if (hitRect.IntersectsWith(tile.BoundBox))
                    {
                        hitSquares.Add(tile);
                    }
                }
        }

        private RectangleF CheckEntityCollisions(List<Collision> blockEntities, CollisionBox hitbox, RectangleF boundbox)
        {
            foreach (GameEntity entity in GameEntity.GetAll())
            {
                if (entity == Parent) continue;
                CollisionComponent coll = entity.GetComponent<CollisionComponent>();
                if (coll == null) continue;

                foreach (CollisionBox targetBox in coll.HitByBoxes(hitbox).Where(box => box.Properties.Blocking))
                {
                    // if he's blocking, check for collision and maybe push me away

                    RectangleF rect = targetBox.BoxAt(coll.PositionSrc.Position);
                    RectangleF adjustrect = rect;
                    adjustrect.X -= Const.PixelEpsilon;
                    adjustrect.Y -= Const.PixelEpsilon;
                    adjustrect.Width += 2 * Const.PixelEpsilon;
                    adjustrect.Height += 2 - Const.PixelEpsilon;
                    RectangleF intersection = RectangleF.Intersect(boundbox, adjustrect);
                    if (intersection.Width != 0 || intersection.Height != 0)
                    {
                        blockEntities.Add(new Collision(hitbox, targetBox, coll));

                        if (hitbox.PushAway)
                        {
                            float vx, vy;
                            MovementComponent mov = entity.GetComponent<MovementComponent>();
                            vx = MovementSrc.VelocityX;
                            vy = MovementSrc.VelocityY;
                            if (mov != null)
                            {
                                vx -= mov.VelocityX;
                                vy -= mov.VelocityY;
                            }

                            PointF offset = hitbox.CheckTileOffset(rect, boundbox, vx, vy, false, false);
                            if (offset.X != 0 || offset.Y != 0)
                            {
                                PositionSrc.Offset(offset.X, offset.Y);
                                boundbox.Offset(offset.X, offset.Y);
                            }
                        }
                    }
                }
            }
            return boundbox;
        }

        // trade in memory to get speed
        private readonly Dictionary<int, List<CollisionBox>> cacheTargetBoxes = new Dictionary<int, List<CollisionBox>>();
        private readonly Dictionary<int, List<CollisionBox>> cacheHitsBoxes = new Dictionary<int, List<CollisionBox>>();

        /// <summary>
        /// Get the hitboxes that the calling box can target
        /// </summary>
        private IEnumerable<CollisionBox> TargetBoxes(CollisionBox hitbox)
        {
            if (!cacheTargetBoxes.ContainsKey(hitbox.ID))
            {
                cacheTargetBoxes[hitbox.ID] = new List<CollisionBox>(hitboxes
                    .Where(box => box.Groups.Intersect(hitbox.Hits).Any()));
            }

            foreach (CollisionBox box in cacheTargetBoxes[hitbox.ID])
            {
                box.SetParent(this);
            }

            return cacheTargetBoxes[hitbox.ID].Where(box => enabledBoxes.Contains(box.ID));
        }

        /// <summary>
        /// Get the hitboxes that the calling box would be targeted by - use for blocking
        /// </summary>
        private IEnumerable<CollisionBox> HitByBoxes(CollisionBox hitbox)
        {
            if (!cacheHitsBoxes.ContainsKey(hitbox.ID))
            {
                cacheHitsBoxes[hitbox.ID] = new List<CollisionBox>(hitboxes
                    .Where(box => box.Hits.Intersect(hitbox.Groups).Any()));
            }

            foreach (CollisionBox box in cacheHitsBoxes[hitbox.ID])
            {
                box.SetParent(this);
            }

            return cacheHitsBoxes[hitbox.ID].Where(box => enabledBoxes.Contains(box.ID));
        }

        public bool TouchedBy(string group)
        {
            return touchedBy.Contains(group);
        }

        public bool TouchedAt(string myGroup, string targetGroup = null)
        {
            if (!touchedAt.ContainsKey(myGroup)) return false;

            if (string.IsNullOrWhiteSpace(targetGroup))
            {
                return touchedAt[myGroup].Count > 0;
            }

            return touchedAt[myGroup].Contains(targetGroup);
        }

        private void Touch(CollisionBox targetbox, CollisionBox mybox)
        {
            foreach (var group in targetbox.Groups)
            {
                touchedBy.Add(group);

                foreach (var mygroup in mybox.Groups)
                {
                    if (!touchedAt.ContainsKey(mygroup))
                    {
                        touchedAt[mygroup] = new HashSet<string>();
                    }
                    touchedAt[mygroup].Add(group);
                }
            }
        }

        private void ReactToTileEffect(TileProperties properties)
        {
            if (MovementSrc != null)
            {
                MovementSrc.PushX(properties.PushX);
                MovementSrc.PushY(properties.PushY);
                MovementSrc.ResistX(properties.ResistX);
                MovementSrc.ResistY(properties.ResistY);
                MovementSrc.DragX(properties.DragX);
                MovementSrc.DragY(properties.DragY);
            }
            // don't just kill, it needs to be conditional on invincibility
            if (properties.Lethal && Parent.Name == "Player") Parent.SendMessage(new DamageMessage(null, float.PositiveInfinity));
        }

        private static RectangleF FloatCorrect(RectangleF rect)
        {
            int rleft = (int)Math.Round(rect.Left);
            int rtop = (int)Math.Round(rect.Top);

            if (Math.Abs(rleft - rect.Left) < Const.PixelEpsilon) rect.X = rleft;
            if (Math.Abs(rtop - rect.Top) < Const.PixelEpsilon) rect.Y = rtop;

            int rright = (int)Math.Round(rect.Right);
            int rbottom = (int)Math.Round(rect.Bottom);

            if (Math.Abs(rright - rect.Right) < Const.PixelEpsilon) rect.Width = rright - rleft;
            if (Math.Abs(rbottom - rect.Bottom) < Const.PixelEpsilon) rect.Height = rbottom - rtop;

            return rect;
        }

        private bool BlockByIntersection(RectangleF myBox, RectangleF targetBox, bool uponly, bool downonly)
        {
            // correct floating point errors
            myBox = FloatCorrect(myBox);
            targetBox = FloatCorrect(targetBox);

            RectangleF intersection = RectangleF.Intersect(myBox, targetBox);
            bool ret = false;

            if (intersection.Height > Const.PixelEpsilon)
            {
                if (intersection.Left == myBox.Left && !downonly && !uponly) BlockLeft = true;
                if (intersection.Right == myBox.Right && !downonly && !uponly) BlockRight = true;
                if (BlockRight)
                {
                    blockRightMin = Math.Min(blockRightMin, intersection.Top);
                    blockRightMax = Math.Max(blockRightMax, intersection.Bottom);
                }
                if (BlockLeft)
                {
                    blockLeftMin = Math.Min(blockLeftMin, intersection.Top);
                    blockLeftMax = Math.Max(blockLeftMax, intersection.Bottom);
                }
                ret = true;
            }
            if (intersection.Width > Const.PixelEpsilon)
            {
                if (intersection.Top == myBox.Top && !downonly) BlockTop = (uponly && MovementSrc != null) ? (MovementSrc.VelocityY * -1 > intersection.Height) : true;
                if (intersection.Bottom == myBox.Bottom && !uponly) BlockBottom = (downonly && MovementSrc != null) ? (MovementSrc.VelocityY > intersection.Height) : true;
                if (BlockBottom)
                {
                    blockBottomMin = Math.Min(blockBottomMin, intersection.Left);
                    blockBottomMax = Math.Max(blockBottomMax, intersection.Right);
                }
                if (BlockTop)
                {
                    blockTopMin = Math.Min(blockTopMin, intersection.Left);
                    blockTopMax = Math.Max(blockTopMax, intersection.Right);
                }
                ret = true;
            }
            return ret;
        }

        private void CollideWith(GameEntity entity, CollisionBox myBox, CollisionBox targetBox)
        {
            float mult = targetBox.DamageMultiplier(Parent.Name);

            float damage = myBox.ContactDamage * mult;

            IGameMessage dmgmsg;
            if (damage > 0) dmgmsg = new DamageMessage(Parent, damage);
            else dmgmsg = new HealMessage(Parent, damage * -1);

            float prevhealth = damage;
            HealthComponent comp = entity.GetComponent<HealthComponent>();
            if (comp != null) prevhealth = comp.Health;

            entity.SendMessage(dmgmsg);
            DamageDealt = Math.Min(damage, prevhealth);
        }

        public static bool VerticalApproach(RectangleF intersection, RectangleF boundBox, float vx, float vy)
        {
            if ((vx == 0 && intersection.Width > 0) || intersection.Height == 0) return true;
            if ((vy == 0 && intersection.Height > 0) || intersection.Width == 0) return false;
            if ((intersection.Bottom == boundBox.Bottom && vy < 0) ||
                (intersection.Top == boundBox.Top && vy > 0))
                return false;
            if ((intersection.Left == boundBox.Left && vx > 0) ||
                (intersection.Right == boundBox.Right && vx < 0))
                return true;
            
            float velocitySlope = Math.Abs(vy) / Math.Abs(vx);
            float collisionSlope = intersection.Height / intersection.Width;
            return (velocitySlope >= collisionSlope);
        }

        public override void RegisterDependencies(Component component)
        {
            if (component is PositionComponent) PositionSrc = component as PositionComponent;
            else if (component is MovementComponent) MovementSrc = component as MovementComponent;
        }

        public static Effect ParseEffect(XElement node)
        {
            Effect effect = entity => {};
            List<CollisionBox> rects = new List<CollisionBox>();
            HashSet<string> enables = new HashSet<string>();
            bool clear = false;

            foreach (XElement prop in node.Elements())
            {
                switch (prop.Name.LocalName)
                {
                    case "Enabled":
                        bool b = prop.GetBool();
                        effect += entity =>
                        {
                            CollisionComponent col = entity.GetComponent<CollisionComponent>();
                            if (col != null) col.Enabled = b;
                        };
                        break;

                    case "Hitbox":
                        rects.Add(new CollisionBox(prop));
                        break;

                    case "EnableBox":
                        XAttribute nameAttrEn = prop.RequireAttribute("name");
                        enables.Add(nameAttrEn.Value);
                        break;

                    case "Clear":
                        clear = true;
                        break;
                }
            }

            if (rects.Count > 0 || enables.Count > 0 || clear) effect += entity =>
            {
                HitBoxMessage msg = new HitBoxMessage(entity, rects, enables, clear);
                entity.SendMessage(msg);
            };

            return effect;
        }
    }
}
