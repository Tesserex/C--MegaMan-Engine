using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    public class CollisionBox : HitBox
    {
        private CollisionComponent parentComponent;

        public int ID { get; private set; }
        public string Name { get; set; }
        public List<string> Hits { get; private set; }
        public List<string> Groups { get; private set; }
        private readonly Dictionary<string, float> resistance;
        public float ContactDamage { get; private set; }

        /// <summary>
        /// Do I block MYSELF when I hit an environment tile?
        /// </summary>
        public bool Environment { get; set; }

        public bool PushAway { get; set; }

        public TileProperties Properties { get; private set; }

        private static int nextID = 0;

        public CollisionBox(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        public CollisionBox(HitBoxInfo info)
            : base(info.Box.X, info.Box.Y, info.Box.Width, info.Box.Height)
        {
            ID = nextID;
            nextID++;

            Name = info.Name;
            ContactDamage = info.ContactDamage;
            Environment = info.Environment;
            PushAway = info.PushAway;

            Hits = new List<string>(info.Hits);
            Groups = new List<string>(info.Groups);
            resistance = new Dictionary<string, float>(info.Resistance);
            Properties = Game.CurrentGame.TileProperties.GetProperties(info.PropertiesName);
        }

        public void SetParent(CollisionComponent parent) { parentComponent = parent; }

        public bool EnvironmentCollisions(PointF position, IMapSquare square, ref PointF offset)
        {
            offset.X = 0;
            offset.Y = 0;
            // some optimizations
            RectangleF tileBox = square.BlockBox;
            if (box.Right + position.X < tileBox.Left) return false;
            if (box.Left + position.X > tileBox.Right) return false;
            RectangleF boundBox = BoxAt(position);

            return EnvironmentContact(square, tileBox, boundBox, out offset);
        }

        private bool EnvironmentContact(IMapSquare square, RectangleF tileBox, RectangleF boundBox, out PointF offset)
        {
            // can't use intersection, use epsilon
            offset = PointF.Empty;
            if (tileBox.Top < boundBox.Top)
            {
                if (tileBox.Bottom - boundBox.Top + Const.PixelEpsilon <= 0) return false;
            }
            else
            {
                if (boundBox.Bottom - tileBox.Top + Const.PixelEpsilon <= 0) return false;
            }
            if (tileBox.Left < boundBox.Left)
            {
                if (tileBox.Right - boundBox.Left + Const.PixelEpsilon <= 0) return false;
            }
            else
            {
                if (boundBox.Right - tileBox.Left + Const.PixelEpsilon <= 0) return false;
            }

            bool down = (!parentComponent.Parent.Container.IsGravityFlipped && square.Properties.Climbable);
            bool up = (parentComponent.Parent.Container.IsGravityFlipped && square.Properties.Climbable);

            if (parentComponent.MovementSrc != null) offset = GetIntersectionOffset(tileBox, boundBox, parentComponent.MovementSrc.VelocityX, parentComponent.MovementSrc.VelocityY, up, down);
            else offset = GetIntersectionOffset(tileBox, boundBox, 0, 0, up, down);

            // Quicksand sinking property tells us not to push the hitbox outward
            if (square.Properties.Sinking > 0)
            {
                // don't clip left or right at all
                offset.X = 0;

                if (parentComponent.Parent.Container.IsGravityFlipped)
                {
                    // don't clip them downward out of the collision
                    if (offset.Y > 0)
                    {
                        offset.Y = 0;
                    }
                }
                else
                {
                    // don't clip them upward out of the collision
                    if (offset.Y < 0)
                    {
                        offset.Y = 0;
                    }
                }
            }

            return true;
        }

        // change those last bools into an enum or something else!
        public PointF GetIntersectionOffset(RectangleF tileBox, RectangleF boundBox, float approach_vx, float approach_vy, bool uponly, bool downonly)
        {
            float top = -1, bottom = -1, left = -1, right = -1;
            RectangleF intersection = RectangleF.Intersect(boundBox, tileBox);

            PointF offset = new PointF(0, 0);
            if (intersection.Width == 0 && intersection.Height == 0) return offset;

            if (Math.Abs(intersection.Bottom - boundBox.Bottom) < Const.PixelEpsilon) bottom = intersection.Height;
            if (Math.Abs(intersection.Top - boundBox.Top) < Const.PixelEpsilon) top = intersection.Height;

            if (Math.Abs(intersection.Right - boundBox.Right) < Const.PixelEpsilon) right = intersection.Width;
            if (Math.Abs(intersection.Left - boundBox.Left) < Const.PixelEpsilon) left = intersection.Width;

            if (top > 0 || bottom > 0 || left > 0 || right > 0)
            {
                bool vert = CollisionComponent.VerticalApproach(intersection, boundBox, approach_vx, approach_vy);

                if (vert)
                {
                    if (downonly)
                    {
                        if (approach_vy > 0 && boundBox.Bottom <= tileBox.Bottom)
                        {
                            offset.Y = -bottom;
                            return offset;
                        }
                        else return new PointF(0, 0);
                    }
                    else if (uponly)
                    {
                        if (approach_vy < 0 && boundBox.Top >= tileBox.Top)
                        {
                            offset.Y = top;
                            return offset;
                        }
                        else return new PointF(0, 0);
                    }

                    if (top >= 0) offset.Y = top;
                    if (bottom >= 0) offset.Y = -bottom;
                }
                else
                {
                    if (uponly || downonly) return new PointF(0, 0);
                    if (left >= 0)
                    {
                        if (approach_vx < 0) offset.X = left;
                    }
                    if (right >= 0)
                    {
                        if (approach_vx > 0) offset.X = -right;
                    }
                }
            }

            return offset;
        }

        public float DamageMultiplier(string entityName)
        {
            if (resistance.ContainsKey(entityName)) return resistance[entityName];
            if (resistance.ContainsKey("ALL")) return resistance["ALL"];
            return 1;
        }

        public RectangleF BoxAt(PointF offset)
        {
            float x = (parentComponent.MovementSrc != null && parentComponent.MovementSrc.Direction == Direction.Left) ? offset.X - box.X - box.Width : box.X + offset.X;

            if (parentComponent.Parent.IsGravitySensitive && parentComponent.Parent.Container.IsGravityFlipped) return new RectangleF(x, offset.Y - box.Y - box.Height, box.Width, box.Height);
            return new RectangleF(x, box.Y + offset.Y, box.Width, box.Height);
        }

        public override RectangleF BoxAt(PointF offset, bool vflip)
        {
            float x = (parentComponent.MovementSrc != null && parentComponent.MovementSrc.Direction == Direction.Left) ? offset.X - box.X - box.Width : box.X + offset.X;

            if (vflip) return new RectangleF(x, offset.Y - box.Y - box.Height, box.Width, box.Height);
            return new RectangleF(x, box.Y + offset.Y, box.Width, box.Height);
        }
    }
}
