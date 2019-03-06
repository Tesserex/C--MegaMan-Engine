using System;
using System.Diagnostics;
using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    [DebuggerDisplay("Parent = {Parent.Name}, Position = ({realX},{realY})")]
    public class PositionComponent : Component
    {
        public bool PersistOffScreen { get; set; }
        public bool IsOffScreen
        {
            get
            {
                return !Parent.Screen.IsOnScreen(X, Y);
            }
        }

        private float realX;
        private float realY;

        public int X { get { return (int)realX; } }
        public int Y { get { return (int)realY; } }

        public Point Position { get { return new Point(X, Y); } }

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

        public void SetX(float x)
        {
            realX = x;
        }

        public void SetY(float y)
        {
            realY = y;
        }

        public void SetPosition(float x, float y)
        {
            realX = x;
            realY = y;
        }

        public void SetPosition(PointF pos)
        {
            SetPosition(pos.X, pos.Y);
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
            realX += x;
            realY += y;
        }

        internal void LoadInfo(PositionComponentInfo info)
        {
            PersistOffScreen = info.PersistOffscreen;
        }
    }
}
