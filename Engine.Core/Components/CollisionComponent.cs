using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;
using MegaMan.Common.Rendering;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    [DebuggerDisplay("Parent = {Parent.Name}, BlockTop: {BlockTop}, BlockLeft: {BlockLeft}, BlockRight: {BlockRight}, BlockBottom: {BlockBottom}")]
    public class CollisionComponent : Component
    {
        private IResourceImage rectTex;

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

        private List<MapSquare> hitSquares;
        private List<Collision> hitBlockEntities;
        
        // Real time is when function to check collision is called function from xml. Collision are checked on call.
        private List<MapSquare> hitSquares_RealTime;
        private List<Collision> hitBlockEntities_RealTime;

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
            var copy = new CollisionComponent
            {
                Enabled = Enabled,
                hitboxes = new List<CollisionBox>()
            };
            foreach (var box in hitboxes) copy.AddBox(box);

            return copy;
        }

        public override void Start(IGameplayContainer container)
        {
            Enabled = true;
            container.GameAct += ClearTouch;
            container.GameReact += Update;
            container.Draw += Instance_GameRender;
        }

        private void ClearTouch()
        {
            touchedBy.Clear();
            touchedAt.Clear();
        }

        void Instance_GameRender(GameRenderEventArgs e)
        {
            if (!Engine.Instance.DrawHitboxes) return;
            foreach (var hitbox in hitboxes)
            {
                if (!enabledBoxes.Contains(hitbox.ID)) continue;

                hitbox.SetParent(this);
                var boundBox = hitbox.BoxAt(PositionSrc.Position);
                boundBox.Offset(-Parent.Screen.OffsetX, -Parent.Screen.OffsetY);

                if (rectTex == null)
                    rectTex = e.RenderContext.CreateColorResource(new Color(1, 0.6f, 0, 0.7f));

                e.RenderContext.Draw(rectTex, 5, new Point((int)(boundBox.X), (int)(boundBox.Y)), new Rectangle(0, 0, (int)(boundBox.Width), (int)(boundBox.Height)));
            }
        }

        public override void Stop(IGameplayContainer container)
        {
            container.GameAct -= ClearTouch;
            container.GameReact -= Update;
            container.Draw -= Instance_GameRender;
            Enabled = false;
            ClearTouch();
            enabledBoxes.Clear();
        }

        public override void Message(IGameMessage msg)
        {
            var boxes = msg as HitBoxMessage;
            if (boxes != null)
            {
                foreach (var box in boxes.AddBoxes)
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
            }
        }

        internal void Loadinfo(CollisionComponentInfo info)
        {
            Enabled = info.Enabled;

            foreach (var box in info.HitBoxes)
            {
                var coll = new CollisionBox(box);
                coll.SetParent(this);
                AddBox(coll);
            }
        }

        public void AddBox(CollisionBox box)
        {
            hitboxes.Add(box);
            if (box.Name != null) boxIDsByName.Add(box.Name, box.ID);
        }

        /// <summary>
        /// Function to check collisions with tiles that occured during tick.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>True if tile collision hit the type received</returns>
        public bool CheckIfOneTileHitContainProperty(string property)
        {
            if (hitSquares == null) return false; // foreach is stupid and will crash if object is null
            return hitSquares.Any(h => CheckTileProperty(h.Properties, property));
        }

        public bool CheckIfHitTileGroup(string group)
        {
            if (hitSquares == null) return false;
            return hitSquares.Any(h => h.Tile.Groups.Contains(group));
        }

        /// <summary>
        /// Function to check collision with solid objects which occured during game tick.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>True if entity collision hit the type received</returns>
        public bool CheckIfOneEntityHitContainProperty(string property)
        {
            if (hitBlockEntities == null) return false; // foreach is stupid and will crash if object is null
            return hitBlockEntities.Any(h => CheckTileProperty(h.targetBox.Properties, property));
        }

        /// <summary>
        /// Function to check RealTime collision (not in game loop). CollisionWithTiles_RealTime Must have been called before.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>True if RealTime tile collision hit the type received</returns>
        public bool CheckIfOneTileHitContainProperty_RealTime(string property)
        {
            if (hitSquares_RealTime == null) return false; // foreach is stupid and will crash if object is null
            return hitSquares_RealTime.Any(h => CheckTileProperty(h.Properties, property));
        }

        public bool CheckIfHitTileGroup_RealTime(string group)
        {
            if (hitSquares_RealTime == null) return false;
            return hitSquares_RealTime.Any(h => h.Tile.Groups.Contains(group));
        }

        /// <summary>
        /// Function to check RealTime collision with solid objects. (not in game loop). CollisionWithAllEntities_RealTime Must have been called before.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>True if RealTime entity collision hit the type received</returns>
        public bool CheckIfOneEntityHitContainProperty_RealTime(string property)
        {
            if (hitBlockEntities_RealTime == null) return false; // foreach is stupid and will crash if object is null
            return hitBlockEntities_RealTime.Any(h => CheckTileProperty(h.targetBox.Properties, property));
        }

        /// <summary>
        /// See returns
        /// </summary>
        /// <param name="tileProperty">Tile property to check</param>
        /// <param name="property">Property to check as a string</param>
        /// <returns>True if property is contained in tileProperty</returns>
        private bool CheckTileProperty(TileProperties tileProperty, string property)
        {
            if (property == "Blocking")
            {
                return tileProperty.Blocking;
            }

            if (property == "Climbable")
            {
                return tileProperty.Climbable;
            }

            if (property == "Lethal")
            {
                return tileProperty.Lethal;
            }

            if (property == "Sinking")
            {
                return (tileProperty.Sinking != 0) ? true : false;
            }
            return false;
        }

        /// <summary>
        /// Function that checks collision with tiles and/or entities.
        /// Stops the checks as soon as one collision is found
        /// </summary>
        /// <param name="boxName">If null, pick every active box of entity picked</param>
        /// <param name="entity">If null, current entity</param>
        /// <param name="property">Example: Blocking</param>
        /// <param name="pushAway">If a collision is found, need to push entity away?</param>
        /// <param name="solidOnly">Applies to entities. Check only solid ones?</param>
        /// <param name="checkTilesForCollisions"></param>
        /// <param name="checkEntitiesForCollisions"></param>
        /// <returns>True if one collision at least</returns>
        /// <remarks>IF MANY ENTITIES, ONLY FIRST ON IN LIST IS CHECKED</remarks>
        public bool CollisionWithAllEntitiesAndTiles_RealTime(string boxName, string entity, string property, bool pushAway, bool solidOnly, bool checkTilesForCollisions, bool checkEntitiesForCollisions)
        {
            var returnVal = false; // As soon as one collision is detected, will always contain true
            var lastFunctionCallReturnValue = false;
            var boxList = CollisionBoxToCheck(boxName, entity);

            var collComponent = (entity == null) ? this : Parent.Entities.GetEntityById(entity)?.GetComponent<CollisionComponent>();
            if (collComponent == null)
                return false;

            foreach (var collisionBox in boxList)
            {
                lastFunctionCallReturnValue = false;

                if (checkTilesForCollisions)
                {
                    lastFunctionCallReturnValue = collComponent.CollisionWithTiles_RealTime(collisionBox, property, pushAway);
                }

                returnVal = (returnVal) ? true : lastFunctionCallReturnValue;

                if (checkEntitiesForCollisions)
                {
                    lastFunctionCallReturnValue = collComponent.CollisionWithAllEntities_RealTime(collisionBox, property, solidOnly, pushAway);
                }

                returnVal = (returnVal) ? true : lastFunctionCallReturnValue;
                if (returnVal) return returnVal;
            }
            return returnVal;
        }

        /// <summary>
        /// Returns list of boxes to check. If boxName is nulled, fetch active boxes of entity, else only return boxName.
        /// </summary>
        /// <param name="boxName"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        private List<CollisionBox> CollisionBoxToCheck(string boxName, string entityId)
        {
            var entityHitboxes = (entityId != null) ? Parent.Entities.GetEntityById(entityId)?.GetComponent<CollisionComponent>()?.hitboxes : hitboxes;
            if (entityHitboxes != null)
            {
                if (boxName != null)
                    return entityHitboxes.Where(h => h.Name == boxName).ToList();
                return entityHitboxes.Where(h => enabledBoxes.Contains(h.ID)).ToList();
            }

            return new List<CollisionBox>();
        }

        /// <summary>
        /// See return value. Fills hitBlockEntities with all/solid (see param solidOnly) entities hit
        /// </summary>
        /// <param name="box">Box for which to check collisions</param>
        /// <param name="property">Property to check for</param>
        /// <param name="solidOnly">Only checks for solid entities</param>
        /// <param name="pushAway">Solve collisions</param>
        /// <returns>True if an entity solid/not (see param solidOnly) is hit</returns>
        private bool CollisionWithAllEntities_RealTime(CollisionBox box, string property, bool solidOnly = true, bool pushAway = false)
        {
            box.SetParent(this);

            var boundbox = box.BoxAt(PositionSrc.Position); //calculate boundbox absolute coordinate
            hitBlockEntities_RealTime = new List<Collision>();

            boundbox = CheckEntityCollisions(hitBlockEntities_RealTime, box, boundbox, pushAway, solidOnly);

            return CheckIfOneEntityHitContainProperty_RealTime(property);
        }

        /// <summary>
        /// See return value. Fills hitSquaresForFunctionThatChecksCollisions with blocks hit
        /// </summary>
        /// <param name="box">Box for which to check collisions</param>
        /// <param name="property">Property to check for</param>
        /// <param name="pushAway">Solve collisions</param>
        /// <returns>True if a solid block is hit</returns>
        private bool CollisionWithTiles_RealTime(CollisionBox box, string property, bool pushAway = false)
        {
            hitSquares_RealTime = new List<MapSquare>(); // hitSquares: those touching
            box.SetParent(this);
            CheckEnvironment(hitSquares_RealTime, box, pushAway);

            return CheckIfOneTileHitContainProperty_RealTime(property);
        }


        protected override void Update()
        {
            hitSquares = hitSquares_RealTime = null;

            DamageDealt = 0;
            BlockTop = BlockRight = BlockLeft = BlockBottom = false;
            blockBottomMin = blockLeftMin = blockRightMin = blockTopMin = float.PositiveInfinity;
            blockBottomMax = blockTopMax = blockLeftMax = blockRightMax = float.NegativeInfinity;
            if (PositionSrc == null) return;
            if (!Enabled) return;

            // first run through, resolve intersections only
            hitSquares = new List<MapSquare>();
            hitBlockEntities = new List<Collision>();

            foreach (var hitbox in hitboxes)
            {
                if (!enabledBoxes.Contains(hitbox.ID)) continue;

                hitbox.SetParent(this);
                if (hitbox.Environment) // check collision with environment
                {
                    CheckEnvironment(hitSquares, hitbox);
                }

                var boundbox = hitbox.BoxAt(PositionSrc.Position);
                hitBlockEntities = new List<Collision>();

                // now check with entity blocks
                if (MovementSrc != null)
                {
                    boundbox = CheckEntityCollisions(hitBlockEntities, hitbox, boundbox);
                }
            }

            // this stores the types of tile we're touching
            var hitTypes = new HashSet<TileProperties>();

            // first, as an aside, if i'm still touching a blocking entity, i need to react to that
            foreach (var collision in hitBlockEntities)
            {
                var boundBox = collision.myBox.BoxAt(PositionSrc.Position);
                var rect = collision.targetBox.BoxAt(collision.targetColl.PositionSrc.Position);
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
            foreach (var hitbox in hitboxes)
            {
                if (!enabledBoxes.Contains(hitbox.ID)) continue;
                ReactForHitbox(hitSquares, hitTypes, hitbox);
            }

            if (MovementSrc != null)
            {
                if (BlockTop && MovementSrc.VelocityY < 0) MovementSrc.VelocityY = 0;
                if (BlockBottom && MovementSrc.VelocityY > 0) MovementSrc.VelocityY = 0;
            }

            // react to tile effects
            foreach (var tileprops in hitTypes)
            {
                ReactToTileEffect(tileprops);
            }
        }

        private void ReactForHitbox(List<MapSquare> hitSquares, HashSet<TileProperties> hitTypes, CollisionBox hitbox)
        {
            hitbox.SetParent(this);

            var boundBox = hitbox.BoxAt(PositionSrc.Position);

            if (hitbox.Environment)
            {
                foreach (var tile in hitSquares)
                {
                    var tileBox = tile.BlockBox;
                    var downonly = (!Parent.Container.IsGravityFlipped && tile.Tile.Properties.Climbable);
                    var uponly = (Parent.Container.IsGravityFlipped && tile.Tile.Properties.Climbable);

                    var hit = (tile.BlockBox != RectangleF.Empty) ? BlockByIntersection(boundBox, tileBox, uponly, downonly) : boundBox.IntersectsWith(tile.BoundBox);

                    if (hitbox.PushAway && (hit || boundBox.IntersectsWith(tile.BoundBox)))    // the environment touched me!
                    {
                        hitTypes.Add(tile.Tile.Properties);
                    }
                }
            }

            // but for entities, we can go ahead and be active aggressors -
            // inflict our effects on the target entity, not the other way around
            foreach (var entity in Parent.Entities.GetAll())
            {
                if (entity == Parent) continue;
                var coll = entity.GetComponent<CollisionComponent>();
                if (coll == null) continue;

                foreach (var targetBox in coll.TargetBoxes(hitbox))
                {
                    boundBox = CheckTargetBox(hitbox, boundBox, entity, coll, targetBox);
                }
            }
        }

        private Rectangle CheckTargetBox(CollisionBox hitbox, Rectangle boundBox, IEntity entity, CollisionComponent coll, CollisionBox targetBox)
        {
            var rect = targetBox.BoxAt(coll.PositionSrc.Position);
            if (boundBox.IntersectsWith(rect))
            {
                coll.Touch(hitbox, targetBox);
                Touch(targetBox, hitbox);
                CollideWith(entity, hitbox, targetBox);
            }
            return boundBox;
        }

        private void CheckEnvironment(List<MapSquare> hitSquares, CollisionBox hitbox, bool pushAway = true)
        {
            var offset = new Point(0, 0);
            var hitRect = hitbox.BoxAt(PositionSrc.Position);    // Absolute positions of collision box

            // this bounds checking prevents needlessly checking collisions way too far away
            // it's a very effective optimization (brings busy time from ~60% down to 45%!)
            var size = Parent.Screen.TileSize;

            for (int y = hitRect.Top - size; y < hitRect.Bottom; y += size)
            {
                for (int x = hitRect.Left - size; x < hitRect.Right; x += size)
                {
                    var tile = Parent.Screen.SquareAt(x, y);
                    if (tile == null) continue;

                    CheckEnvironmentTile(hitSquares, hitbox, hitRect, tile, ref offset, pushAway);
                }

                var rightEdge = Parent.Screen.SquareAt(hitRect.Right, y);
                if (rightEdge != null)
                {
                    CheckEnvironmentTile(hitSquares, hitbox, hitRect, rightEdge, ref offset, pushAway);
                }
            }

            for (int x = hitRect.Left - size; x < hitRect.Right; x += size)
            {
                var tile = Parent.Screen.SquareAt(x, hitRect.Bottom);
                if (tile == null) continue;

                CheckEnvironmentTile(hitSquares, hitbox, hitRect, tile, ref offset, pushAway);
            }

            var lastCorner = Parent.Screen.SquareAt(hitRect.Right, hitRect.Bottom);
            if (lastCorner != null)
            {
                CheckEnvironmentTile(hitSquares, hitbox, hitRect, lastCorner, ref offset, pushAway);
            }
        }

        private void CheckEnvironmentTile(List<MapSquare> hitSquares, CollisionBox hitbox, Rectangle hitRect, MapSquare tile, ref Point offset, bool pushAway)
        {
            if (hitbox.EnvironmentCollisions(PositionSrc.Position, tile, ref offset))
            {
                hitSquares.Add(tile);
                if (hitbox.PushAway && pushAway) PositionSrc.Offset(offset.X, offset.Y);
            }
            else if (hitRect.IntersectsWith(tile.BoundBox))
            {
                hitSquares.Add(tile);
            }
        }

        private Rectangle CheckEntityCollisions(List<Collision> blockEntities, CollisionBox hitbox, Rectangle boundbox, bool pushAway = true, bool solidOnly = true)
        {
            foreach (var entity in Parent.Entities.GetAll())
            {
                if (entity == Parent) continue;
                var coll = entity.GetComponent<CollisionComponent>();
                if (coll == null) continue;

                var collToCheck = (solidOnly) ? coll.HitByBoxes(hitbox).Where(box => box.Properties.Blocking) : coll.HitByBoxes(hitbox);

                foreach (var targetBox in collToCheck)
                {
                    // if he's blocking, check for collision and maybe push me away

                    Rectangle rect = targetBox.BoxAt(coll.PositionSrc.Position);
                    Rectangle intersection = Rectangle.Intersect(boundbox, rect);
                    if (intersection.Width != 0 || intersection.Height != 0)
                    {
                        blockEntities.Add(new Collision(hitbox, targetBox, coll));

                        if (hitbox.PushAway && pushAway)
                        {
                            float vx, vy;
                            var mov = entity.GetComponent<MovementComponent>();
                            vx = MovementSrc.VelocityX;
                            vy = MovementSrc.VelocityY;
                            if (mov != null)
                            {
                                vx -= mov.VelocityX;
                                vy -= mov.VelocityY;
                            }

                            var offset = hitbox.GetIntersectionOffset(rect, boundbox, vx, vy, false, false);
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

            foreach (var box in cacheTargetBoxes[hitbox.ID])
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

            foreach (var box in cacheHitsBoxes[hitbox.ID])
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

                if (properties.Sinking > 0)
                {
                    if (Parent.Container.IsGravityFlipped)
                    {
                        if (MovementSrc.VelocityY <= 0)
                        {
                            BlockTop = true;
                            PositionSrc.Offset(0, -1 * properties.Sinking);
                            // don't let gravity accumulate like in MM1
                            MovementSrc.VelocityY = 0;
                        }
                    }
                    else
                    {
                        if (MovementSrc.VelocityY >= 0)
                        {
                            BlockBottom = true;
                            PositionSrc.Offset(0, properties.Sinking);
                            MovementSrc.VelocityY = 0;
                        }
                    }
                }
            }
            // don't just kill, it needs to be conditional on invincibility
            if (properties.Lethal && Parent.Name == "Player") Parent.SendMessage(new DamageMessage(null, float.PositiveInfinity));
        }

        private bool BlockByIntersection(Rectangle myBox, Rectangle targetBox, bool uponly, bool downonly)
        {
            var intersection = Rectangle.Intersect(myBox, targetBox);
            bool ret = false;

            if (intersection.Height > 0)
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
            if (intersection.Width > 0)
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

        private void CollideWith(IEntity entity, CollisionBox myBox, CollisionBox targetBox)
        {
            var mult = targetBox.DamageMultiplier(Parent.Name);

            var damage = myBox.ContactDamage * mult;

            IGameMessage dmgmsg;
            if (damage > 0) dmgmsg = new DamageMessage(Parent, damage);
            else dmgmsg = new HealMessage(Parent, damage * -1);

            var prevhealth = damage;
            var comp = entity.GetComponent<HealthComponent>();
            if (comp != null) prevhealth = comp.Health;

            entity.SendMessage(dmgmsg);
            DamageDealt = Math.Min(damage, prevhealth);
        }

        public static bool VerticalApproach(Rectangle intersection, Rectangle boundBox, float vx, float vy)
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
    }
}
