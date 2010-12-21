using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace Mega_Man
{
    [System.Diagnostics.DebuggerDisplay("Parent = {Parent.Name}, BlockTop: {BlockTop}, BlockLeft: {BlockLeft}, BlockRight: {BlockRight}, BlockBottom: {BlockBottom}")]
    public class CollisionComponent : Component
    {
        private Texture2D rectTex;

        private class Collision
        {
            public CollisionBox myBox;
            public CollisionComponent targetColl;
            public CollisionBox targetBox;

            public Collision(CollisionBox mybox, CollisionBox target, CollisionComponent tgtComp)
            {
                myBox = mybox;
                targetBox = target;
                targetColl = tgtComp;
            }
        }

        private List<CollisionBox> hitboxes = new List<CollisionBox>();
        private List<string> touchedBy = new List<string>();
        private HashSet<string> enabledBoxes = new HashSet<string>();

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
            CollisionComponent copy = new CollisionComponent();
            copy.Enabled = this.Enabled;
            copy.hitboxes = new List<CollisionBox>();
            foreach (CollisionBox box in this.hitboxes) copy.hitboxes.Add(box);

            return copy;
        }

        public override void Start()
        {
            Enabled = true;
            Engine.Instance.GameAct += ClearTouch;
            Engine.Instance.GameReact += Update;
            Engine.Instance.GameRender += new GameRenderEventHandler(Instance_GameRender);

            rectTex = new Texture2D(Engine.Instance.GraphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            rectTex.SetData(new Microsoft.Xna.Framework.Graphics.Color[] { new Microsoft.Xna.Framework.Graphics.Color(1, 0.6f, 0, 0.7f) });
        }

        private void ClearTouch()
        {
            this.touchedBy.Clear();
        }

        void Instance_GameRender(GameRenderEventArgs e)
        {
            if (!Engine.Instance.DrawHitboxes) return;
            foreach (CollisionBox hitbox in hitboxes)
            {
                if (!enabledBoxes.Contains(hitbox.Name)) continue;

                System.Drawing.RectangleF boundBox = hitbox.BoxAt(PositionSrc.Position);
                boundBox.Offset(-Game.CurrentGame.CurrentMap.CurrentScreen.OffsetX, -Game.CurrentGame.CurrentMap.CurrentScreen.OffsetY);
                e.Layers.ForegroundBatch.Draw(rectTex, new Microsoft.Xna.Framework.Rectangle((int)(boundBox.X), (int)(boundBox.Y), (int)(boundBox.Width), (int)(boundBox.Height)), Microsoft.Xna.Framework.Graphics.Color.White);
            }
        }

        public override void Stop()
        {
            Engine.Instance.GameAct -= ClearTouch;
            Engine.Instance.GameReact -= Update;
            Engine.Instance.GameRender -= new GameRenderEventHandler(Instance_GameRender);
        }

        public override void Message(IGameMessage msg)
        {
            HitBoxMessage boxes = msg as HitBoxMessage;
            if (boxes != null)
            {
                if (boxes.Clear) hitboxes.Clear();

                foreach (CollisionBox box in boxes.AddBoxes)
                {
                    box.SetParent(this);
                    hitboxes.Add(box);
                }

                enabledBoxes.Clear();
                foreach (var name in boxes.EnableBoxes) enabledBoxes.Add(name);

                return;
            }
        }

        public override void LoadXml(XElement xml)
        {
            foreach (XElement boxnode in xml.Elements("Hitbox"))
            {
                CollisionBox box = new CollisionBox(boxnode);
                box.SetParent(this);
                hitboxes.Add(box);
            }
            XElement blockNode = xml.Element("Enabled");
            if (blockNode != null)
            {
                bool b;
                if (!bool.TryParse(blockNode.Value, out b)) throw new EntityXmlException(blockNode, "Enabled tag must contain a bool (true or false).");
                this.Enabled = b;
            }
        }

        protected override void Update()
        {
            if (Parent.Paused) return;

            DamageDealt = 0;
            BlockTop = BlockRight = BlockLeft = BlockBottom = false;
            blockBottomMin = blockLeftMin = blockRightMin = blockTopMin = float.PositiveInfinity;
            blockBottomMax = blockTopMax = blockLeftMax = blockRightMax = float.NegativeInfinity;
            if (this.PositionSrc == null) return;
            if (!Enabled) return;

            // first run through, resolve intersections only
            List<MapSquare> hitSquares = new List<MapSquare>();
            List<Collision> blockEntities = new List<Collision>();
            foreach (CollisionBox hitbox in this.hitboxes)
            {
                if (!enabledBoxes.Contains(hitbox.Name)) continue;

                hitbox.SetParent(this);
                if (hitbox.Environment) // check collision with environment
                {
                    PointF offset = new PointF(0, 0);
                    RectangleF hitRect = hitbox.BoxAt(PositionSrc.Position);

                    // this bounds checking prevents needlessly checking collisions way too far away
                    // it's a very effective optimization (brings busy time from ~60% down to 45%!)
                    int size = Parent.Screen.Screen.Tileset.TileSize;
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

                RectangleF boundbox = hitbox.BoxAt(PositionSrc.Position);

                // now check with entity blocks
                foreach (GameEntity entity in GameEntity.GetAll())
                {
                    if (entity == this.Parent) continue;
                    CollisionComponent coll = entity.GetComponent<CollisionComponent>();
                    if (coll == null) continue;

                    foreach (CollisionBox targetBox in coll.HitByBoxes(hitbox.Groups))
                    {
                        // if he's blocking, check for collision and maybe push me away
                        if (targetBox.Properties.Blocking)
                        {
                            RectangleF rect = targetBox.BoxAt(coll.PositionSrc.Position);
                            RectangleF adjustrect = rect;
                            adjustrect.X -= Const.PixelEpsilon;
                            adjustrect.Y -= Const.PixelEpsilon;
                            adjustrect.Width += 2 * Const.PixelEpsilon;
                            adjustrect.Height += 2 - Const.PixelEpsilon;
                            RectangleF intersection = RectangleF.Intersect(boundbox, adjustrect);
                            if ((intersection.Width != 0 || intersection.Height != 0) && MovementSrc != null && hitbox.PushAway) // because we don't want to push stationary things like tiles
                            {
                                blockEntities.Add(new Collision(hitbox, targetBox, coll));
                                float vx = 0, vy = 0;
                                if (MovementSrc != null)
                                {
                                    MovementComponent mov = entity.GetComponent<MovementComponent>();
                                    vx = MovementSrc.VelocityX;
                                    vy = MovementSrc.VelocityY;
                                    if (mov != null)
                                    {
                                        vx -= mov.VelocityX;
                                        vy -= mov.VelocityY;
                                    }
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
            }

            // this stores the types of tile we're touching
            HashSet<MegaMan.TileProperties> hitTypes = new HashSet<MegaMan.TileProperties>();

            // first, as an aside, if i'm still touching a blocking entity, i need to react to that
            foreach (Collision collision in blockEntities)
            {
                RectangleF boundBox = collision.myBox.BoxAt(PositionSrc.Position);
                RectangleF rect = collision.targetBox.BoxAt(collision.targetColl.PositionSrc.Position);
                if (this.BlockByIntersection(boundBox, rect, false, false))
                {
                    collision.targetColl.Touch(collision.myBox);
                    // for now, entities can only be normal type
                    hitTypes.Add(collision.targetBox.Properties);
                    // now cause friction on the x, a la moving platforms
                    if (collision.targetColl.MovementSrc != null) MovementSrc.PushX(collision.targetColl.MovementSrc.VelocityX);
                }
            }

            // now determine who i'm still touching, the resulting effects
            // notice - there's really no one to speak on behalf of the environment,
            // so we need to inflict the effects upon ourself
            foreach (CollisionBox hitbox in this.hitboxes)
            {
                if (!enabledBoxes.Contains(hitbox.Name)) continue;
                hitbox.SetParent(this);

                System.Drawing.RectangleF boundBox = hitbox.BoxAt(PositionSrc.Position);

                if (hitbox.Environment)
                {
                    foreach (MapSquare tile in hitSquares)
                    {
                        RectangleF tileBox = tile.BlockBox;
                        bool downonly = (!Game.CurrentGame.GravityFlip && tile.Tile.Properties.Climbable);
                        bool uponly = (Game.CurrentGame.GravityFlip && tile.Tile.Properties.Climbable);

                        bool hit = (tile.BlockBox != RectangleF.Empty && hitbox.PushAway)? this.BlockByIntersection(boundBox, tileBox, uponly, downonly) : boundBox.IntersectsWith(tile.BoundBox);

                        if (hit || boundBox.IntersectsWith(tile.BoundBox))    // the environment touched me!
                        {
                            hitTypes.Add(tile.Tile.Properties);
                        }
                    }
                }

                // but for entities, we can go ahead and be active aggressors -
                // inflict our effects on the target entity, not the other way around
                foreach (GameEntity entity in GameEntity.GetAll())
                {
                    if (entity == this.Parent) continue;
                    CollisionComponent coll = entity.GetComponent<CollisionComponent>();
                    if (coll == null) continue;

                    foreach (CollisionBox targetBox in coll.TargetBoxes(hitbox.Hits))
                    {
                        RectangleF rect = targetBox.BoxAt(coll.PositionSrc.Position);
                        if (boundBox.IntersectsWith(rect))
                        {
                            coll.Touch(hitbox);
                            this.Touch(targetBox);
                            this.CollideWith(entity, hitbox, targetBox);
                        }
                    }
                }
            }
            if (MovementSrc != null)
            {
                if (BlockTop && MovementSrc.VelocityY < 0) MovementSrc.VelocityY = 0;
                if (BlockLeft && MovementSrc.VelocityX < 0) MovementSrc.VelocityX = 0;
                if (BlockRight && MovementSrc.VelocityX > 0) MovementSrc.VelocityX = 0;
                if (BlockBottom && MovementSrc.VelocityY > 0) MovementSrc.VelocityY = 0;
            }

            // react to tile effects
            foreach (MegaMan.TileProperties tileprops in hitTypes)
            {
                ReactToTileEffect(tileprops);
            }
        }

        /// <summary>
        /// Get the hitboxes that the calling box can target
        /// </summary>
        public IEnumerable<CollisionBox> TargetBoxes(IEnumerable<string> hitGroups)
        {
            foreach (CollisionBox box in this.hitboxes)
            {
                if (!enabledBoxes.Contains(box.Name)) continue;
                box.SetParent(this);

                foreach (var a in box.Groups.Intersect(hitGroups))
                {
                    yield return box;
                    break;
                }
            }
        }

        /// <summary>
        /// Get the hitboxes that the calling box would be targeted by - use for blocking
        /// </summary>
        public IEnumerable<CollisionBox> HitByBoxes(IEnumerable<string> targetGroups)
        {
            foreach (CollisionBox box in this.hitboxes)
            {
                if (!enabledBoxes.Contains(box.Name)) continue;

                box.SetParent(this);
                foreach (var a in box.Hits.Intersect(targetGroups))
                {
                    yield return box;
                    break;
                }
            }
        }

        public bool TouchedBy(string group)
        {
            return touchedBy.Contains(group);
        }

        private void Touch(CollisionBox box)
        {
            foreach (string group in box.Groups)
            {
                this.touchedBy.Add(group);
            }
        }

        private void ReactToTileEffect(MegaMan.TileProperties properties)
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
            if (properties.Lethal && this.Parent.Name == "Player") this.Parent.SendMessage(new DamageMessage(null, float.PositiveInfinity));
        }

        private RectangleF FloatCorrect(RectangleF rect)
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

        private bool CollideWith(GameEntity entity, CollisionBox myBox, CollisionBox targetBox)
        {
            float mult = targetBox.DamageMultiplier(this.Parent.Name);

            float damage = myBox.ContactDamage * mult;

            IGameMessage dmgmsg;
            if (damage > 0) dmgmsg = new DamageMessage(Parent, damage);
            else dmgmsg = new HealMessage(Parent, damage * -1);

            float prevhealth = damage;
            HealthComponent comp = entity.GetComponent<HealthComponent>();
            if (comp != null) prevhealth = comp.Health;

            entity.SendMessage(dmgmsg);
            DamageDealt = Math.Min(damage, prevhealth);

            return false;
        }

        public static bool VerticalApproach(RectangleF intersection, RectangleF boundBox, float vx, float vy)
        {
            if ((vx == 0 && intersection.Width > 0) || intersection.Height == 0) return true;
            else if ((vy == 0 && intersection.Height > 0) || intersection.Width == 0) return false;
            else if ((intersection.Bottom == boundBox.Bottom && vy < 0) ||
                (intersection.Top == boundBox.Top && vy > 0))
                return false;
            else if ((intersection.Left == boundBox.Left && vx > 0) ||
                (intersection.Right == boundBox.Right && vx < 0))
                return true;
            else
            {
                float velocitySlope = Math.Abs(vy) / Math.Abs(vx);
                float collisionSlope = intersection.Height / intersection.Width;
                return (velocitySlope >= collisionSlope);
            }
        }

        public override void RegisterDependencies(Component component)
        {
            if (component is PositionComponent) this.PositionSrc = component as PositionComponent;
            else if (component is MovementComponent) this.MovementSrc = component as MovementComponent;
        }

        public override Effect ParseEffect(XElement node)
        {
            Effect effect = (entity) => {};
            List<CollisionBox> rects = new List<CollisionBox>();
            HashSet<string> enables = new HashSet<string>();
            bool clear = false;

            foreach (XElement prop in node.Elements())
            {
                switch (prop.Name.LocalName)
                {
                    case "Enabled":
                        bool b;
                        if (!bool.TryParse(prop.Value, out b)) throw new EntityXmlException(prop, "Enabled value could not be parse as a boolean (true or false).");
                        effect += (entity) =>
                        {
                            CollisionComponent col = entity.GetComponent<CollisionComponent>();
                            if (col != null) col.Enabled = b;
                        };
                        break;

                    case "Hitbox":
                        rects.Add(new CollisionBox(prop));
                        break;

                    case "EnableBox":
                        XAttribute nameAttrEn = prop.Attribute("name");
                        if (nameAttrEn == null) throw new EntityXmlException(prop, "Collision EnableBox tag must have a name attribute!");
                        enables.Add(nameAttrEn.Value);
                        break;

                    case "Clear":
                        clear = true;
                        break;
                }
            }

            if (rects.Count > 0 || enables.Count > 0 || clear) effect += (entity) =>
            {
                HitBoxMessage msg = new HitBoxMessage(entity, rects, enables, clear);
                entity.SendMessage(msg);
            };

            return effect;
        }
    }
}
