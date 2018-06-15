using System;

namespace MegaMan.Common
{
    public enum RespawnBehavior
    {
        Offscreen,
        Death,
        Stage,
        Never
    }

    public class EntityPlacement
    {
        private string fixedId;
        private string guidId;

        public string Id
        {
            get
            {
                if (fixedId != null)
                    return fixedId;

                if (guidId == null)
                    guidId = Guid.NewGuid().ToString();

                return guidId;
            }
            set
            {
                fixedId = value;
            }
        }

        public string Entity;
        public string State;
        public Direction Direction;
        public RespawnBehavior Respawn;
        public int ScreenX;
        public int ScreenY;

        public override bool Equals(object obj)
        {
            if (obj is EntityPlacement)
                return ((EntityPlacement)obj).Id == Id;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public EntityPlacement Clone()
        {
            return new EntityPlacement {
                Entity = Entity,
                Direction = Direction,
                State = State,
                Respawn = Respawn,
                ScreenX = ScreenX,
                ScreenY = ScreenY
            };
        }
    }
}
