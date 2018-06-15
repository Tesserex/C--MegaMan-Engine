using System;
using System.Diagnostics;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    [DebuggerDisplay("Parent = {Parent.Name}, Position = {Position}")]
    public class PositionComponent : Component
    {
        public bool PersistOffScreen { get; set; }
        public PointF Position { get; private set; }
        public bool IsOffScreen
        {
            get
            {
                return !Parent.Screen.IsOnScreen(Position.X, Position.Y);
            }
        }

        public float X { get { return Position.X; } }
        public float Y { get { return Position.Y; } }

        public override Component Clone()
        {
            var copy = new PositionComponent {PersistOffScreen = PersistOffScreen};
            return copy;
        }

        public override void Start(IGameplayContainer container)
        {
            container.GameCleanup += Update;
        }

        public override void Stop(IGameplayContainer container)
        {
            container.GameCleanup -= Update;
        }

        public override void Message(IGameMessage msg)
        {
            
        }

        public void SetPosition(float x, float y)
        {
            SetPosition(new PointF(x, y));
        }

        public void SetPosition(PointF pos)
        {
            // fix float errors by rounding
            pos.X = (float)Math.Round(pos.X, 3);
            pos.Y = (float)Math.Round(pos.Y, 3);
            Position = pos;
        }

        protected override void Update()
        {
            if (!PersistOffScreen && IsOffScreen && Parent.Name != "Player")
            {
                Parent.Remove();
            }
        }

        public override void RegisterDependencies(Component component)
        {
            
        }

        public void Offset(float x, float y)
        {
            Position = new PointF(Position.X + x, Position.Y + y);
        }

        internal void LoadInfo(PositionComponentInfo info)
        {
            PersistOffScreen = info.PersistOffscreen;
        }
    }
}
