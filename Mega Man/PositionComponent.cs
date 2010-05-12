using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;

namespace Mega_Man
{
    [System.Diagnostics.DebuggerDisplay("Parent = {Parent.Name}, Position = {Position}")]
    public class PositionComponent : Component, IPositioned
    {
        public bool PersistOffScreen { get; set; }
        public IMovement MovementSrc { get; private set; }
        public PointF Position { get; private set; }
        public bool IsOffScreen
        {
            get
            {
                return !Game.CurrentGame.CurrentMap.IsOnScreen(Position.X, Position.Y);
            }
        }

        public PositionComponent()
        {
            Position = new PointF(48, 128);
        }

        public override Component Clone()
        {
            PositionComponent copy = new PositionComponent();
            copy.PersistOffScreen = this.PersistOffScreen;
            return copy;
        }

        public override void Start()
        {
            Engine.Instance.GameCleanup += Update;
        }

        public override void Stop()
        {
            Engine.Instance.GameCleanup -= Update;
        }

        public override void Message(IGameMessage msg)
        {
            
        }

        public void SetPosition(PointF pos)
        {
            Position = pos;
        }

        protected override void Update()
        {
            if (!PersistOffScreen && IsOffScreen && Parent.Name != "Player")
            {
                Parent.Stop();
                return;
            }
        }

        public override void RegisterDependencies(Component component)
        {
            if (component is IMovement) MovementSrc = component as IMovement;
        }

        public void Offset(float x, float y)
        {
            Position = new PointF(Position.X + x, Position.Y + y);
        }

        public override void LoadXml(XElement node)
        {
            XAttribute persistAttr = node.Attribute("persistoffscreen");
            if (persistAttr != null)
            {
                bool p;
                if (bool.TryParse(persistAttr.Value, out p)) PersistOffScreen = p;
            }
        }

        public override Effect ParseEffect(XElement child)
        {
            Effect action = new Effect((entity) => { });
            foreach (XElement prop in child.Elements())
            {
                switch (prop.Name.LocalName)
                {
                    case "X":
                        action += ParsePositionBehavior(prop, Axis.X);
                        break;

                    case "Y":
                        action += ParsePositionBehavior(prop, Axis.Y);
                        break;
                }
            }
            return action;
        }

        public static Effect ParsePositionBehavior(XElement prop, Axis axis)
        {
            Effect action = (e) => { };

            XAttribute baseAttr = prop.Attribute("base");
            if (baseAttr != null)
            {
                string base_str = baseAttr.Value;
                if (base_str == "Inherit")
                {
                    action = (entity) =>
                    {
                        PositionComponent pos = entity.GetComponent<PositionComponent>();
                        if (pos != null && entity.Parent != null)
                        {
                            PositionComponent parentPos = entity.Parent.GetComponent<PositionComponent>();
                            if (parentPos != null)
                            {
                                if (axis == Axis.X) pos.SetPosition(new System.Drawing.PointF(parentPos.Position.X, pos.Position.Y));
                                else pos.SetPosition(new System.Drawing.PointF(pos.Position.X, parentPos.Position.Y));
                            }
                        }
                    };
                }
                else
                {
                    float baseVal;
                    if (!float.TryParse(base_str, out baseVal)) throw new EntityXmlException(baseAttr, "Position base must be either \"Inherit\" or a valid decimal number.");
                    if (axis == Axis.X) action = (entity) =>
                    {
                        PositionComponent pos = entity.Parent.GetComponent<PositionComponent>();
                        if (pos != null) pos.SetPosition(new System.Drawing.PointF(baseVal, pos.Position.Y));
                    };
                    else action = (entity) =>
                    {
                        PositionComponent pos = entity.Parent.GetComponent<PositionComponent>();
                        if (pos != null) pos.SetPosition(new System.Drawing.PointF(pos.Position.X, baseVal));
                    };
                }
            }

            XAttribute offattr = prop.Attribute("offset");
            if (offattr != null)
            {
                float offset;
                if (!float.TryParse(offattr.Value, out offset)) throw new EntityXmlException(offattr, "Position offset must be a valid decimal number.");
                XAttribute offdirattr = prop.Attribute("direction");
                if (offdirattr == null) throw new EntityXmlException(prop, "X position specifies offset but no direction! Please specify a \"direction\" attribute.");
                if (offdirattr.Value == "Inherit")
                {
                    action += (entity) =>
                    {
                        PositionComponent pos = entity.GetComponent<PositionComponent>();
                        if (pos != null && entity.Parent != null)
                        {
                            Direction offdir = entity.Parent.Direction;
                            switch (offdir)
                            {
                                case Direction.Down: pos.SetPosition(new System.Drawing.PointF(pos.Position.X, pos.Position.Y + offset)); break;
                                case Direction.Up: pos.SetPosition(new System.Drawing.PointF(pos.Position.X, pos.Position.Y - offset)); break;
                                case Direction.Left: pos.SetPosition(new System.Drawing.PointF(pos.Position.X - offset, pos.Position.Y)); break;
                                case Direction.Right: pos.SetPosition(new System.Drawing.PointF(pos.Position.X + offset, pos.Position.Y)); break;
                            }
                        }
                    };
                }
                else if (offdirattr.Value == "Input")
                {
                    action += (entity) =>
                    {
                        PositionComponent pos = entity.GetComponent<PositionComponent>();
                        InputComponent input = entity.GetComponent<InputComponent>();
                        if (input != null && pos != null)
                        {
                            if (axis == Axis.Y)
                            {
                                if (input.Down) pos.SetPosition(new System.Drawing.PointF(pos.Position.X, pos.Position.Y + offset));
                                else if (input.Up) pos.SetPosition(new System.Drawing.PointF(pos.Position.X, pos.Position.Y - offset));
                            }
                            else
                            {
                                if (input.Left) pos.SetPosition(new System.Drawing.PointF(pos.Position.X - offset, pos.Position.Y));
                                else if (input.Right || (!input.Up && !input.Down)) pos.SetPosition(new System.Drawing.PointF(pos.Position.X + offset, pos.Position.Y));
                            }
                        }
                    };
                }
                else
                {
                    Direction offdir;
                    try
                    {
                        offdir = (Direction)Enum.Parse(typeof(Direction), offdirattr.Value, true);
                    }
                    catch
                    {
                        throw new EntityXmlException(offdirattr, "Position offset direction was not valid!");
                    }
                    switch (offdir)
                    {
                        case Direction.Left: action += (entity) =>
                        {
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (pos != null) pos.SetPosition(new System.Drawing.PointF(pos.Position.X - offset, pos.Position.Y));
                        };
                            break;
                        case Direction.Right: action += (entity) =>
                        {
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (pos != null) pos.SetPosition(new System.Drawing.PointF(pos.Position.X + offset, pos.Position.Y));
                        };
                            break;
                        case Direction.Down: action += (entity) =>
                        {
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (pos != null) pos.SetPosition(new System.Drawing.PointF(pos.Position.X, pos.Position.Y + offset));
                        };
                            break;
                        case Direction.Up: action += (entity) =>
                        {
                            PositionComponent pos = entity.GetComponent<PositionComponent>();
                            if (pos != null) pos.SetPosition(new System.Drawing.PointF(pos.Position.X, pos.Position.Y - offset));
                        };
                            break;
                    }
                }
            }
            return action;
        }
    }
}
