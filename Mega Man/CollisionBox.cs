using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Drawing;

namespace Mega_Man
{
    public class CollisionBox : HitBox
    {
        private CollisionComponent parent;

        public List<string> Hits { get; private set; }
        public List<string> Groups { get; private set; }
        private Dictionary<string, float> resistance;
        public float ContactDamage { get; private set; }

        /// <summary>
        /// Do I block MYSELF when I hit an environment tile?
        /// </summary>
        public bool Environment { get; private set; }

        public bool PushAway { get; private set; }

        public MegaMan.TileProperties Properties { get; private set; }

        public CollisionBox(XElement xmlNode)
            : base(xmlNode)
        {
            Hits = new List<string>();
            Groups = new List<string>();
            resistance = new Dictionary<string, float>();
            Properties = MegaMan.TileProperties.Default;

            foreach (XElement groupnode in xmlNode.Elements("Hits"))
            {
                Hits.Add(groupnode.Value);
            }

            foreach (XElement groupnode in xmlNode.Elements("Group"))
            {
                Groups.Add(groupnode.Value);
            }

            foreach (XElement resistNode in xmlNode.Elements("Resist"))
            {
                XAttribute nameAttr = resistNode.Attribute("name");
                if (nameAttr == null) throw new EntityXmlException(resistNode, "Resist tag must specify an entity name as a name attribute!");

                XAttribute multAttr = resistNode.Attribute("multiply");
                if (multAttr == null) throw new EntityXmlException(resistNode, "Resist tag mult specify a multiply attribute!");

                float mult;
                if (!float.TryParse(multAttr.Value, out mult)) throw new EntityXmlException(multAttr, "Multiply value is not a valid number!");

                resistance.Add(nameAttr.Value, mult);
            }

            XAttribute damageAttr = xmlNode.Attribute("damage");
            if (damageAttr != null)
            {
                float dmg;
                bool s = float.TryParse(damageAttr.Value, out dmg);
                if (!s) throw new EntityXmlException(damageAttr, "Hitbox damage value is not a valid number.");
                this.ContactDamage = dmg;
            }

            bool env = true;
            XAttribute envAttr = xmlNode.Attribute("environment");
            if (envAttr != null)
            {
                if (!bool.TryParse(envAttr.Value, out env)) throw new EntityXmlException(envAttr, "Hitbox environment attribute is not a valid boolean (true or false).");
            }
            Environment = env;

            bool push = true;
            XAttribute pushAttr = xmlNode.Attribute("pushaway");
            if (pushAttr != null)
            {
                if (!bool.TryParse(pushAttr.Value, out push)) throw new EntityXmlException(envAttr, "Hitbox pushaway attribute is not a valid boolean (true or false).");
            }
            PushAway = push;

            XAttribute propAttr = xmlNode.Attribute("properties");
            if (propAttr != null) this.Properties = GameEntity.GetProperties(propAttr.Value);
        }

        public void SetParent(CollisionComponent parent) { this.parent = parent; }

        public bool EnvironmentCollisions(PointF position, MapSquare tile, ref PointF offset)
        {
            offset.X = 0;
            offset.Y = 0;
            // some optimizations
            RectangleF tileBox = tile.BlockBox;
            if (this.box.Right + position.X < tileBox.Left) return false;
            if (this.box.Left + position.X > tileBox.Right) return false;
            System.Drawing.RectangleF boundBox = this.BoxAt(position, parent.Parent.GravityFlip ? Game.CurrentGame.GravityFlip : false);

            return EnvironmentContact(tile, tileBox, boundBox, out offset);
        }

        private bool EnvironmentContact(MapSquare square, RectangleF tileBox, RectangleF boundBox, out PointF offset)
        {
            RectangleF inter = RectangleF.Intersect(tileBox, boundBox);
            if (inter.Height == 0 && inter.Width == 0)
            {
                offset = new PointF(0, 0);
                return false;
            }

            bool down = (!Game.CurrentGame.GravityFlip && square.Tile.Properties.Climbable);
            bool up = (Game.CurrentGame.GravityFlip && square.Tile.Properties.Climbable);

            if (parent.MovementSrc != null) offset = CheckTileOffset(tileBox, boundBox, parent.MovementSrc.VelocityX, parent.MovementSrc.VelocityY, up, down);
            else offset = CheckTileOffset(tileBox, boundBox, 0, 0, up, down);
            return true;
        }

        // change those last bools into an enum or something else!
        public PointF CheckTileOffset(RectangleF tileBox, RectangleF boundBox, float approach_vx, float approach_vy, bool uponly, bool downonly)
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
            float x = (parent.MovementSrc != null && parent.MovementSrc.Direction == Direction.Left) ? offset.X - box.X - box.Width : box.X + offset.X;

            if (parent.Parent.GravityFlip && Game.CurrentGame.GravityFlip) return new System.Drawing.RectangleF(x, offset.Y - box.Y - box.Height, box.Width, box.Height);
            return new System.Drawing.RectangleF(x, box.Y + offset.Y, box.Width, box.Height);
        }
    }
}
