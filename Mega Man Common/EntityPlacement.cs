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
        private string _fixedId;
        private string _guidId;

        public string Id
        {
            get
            {
                if (_fixedId != null)
                    return _fixedId;

                if (_guidId == null)
                    _guidId = Guid.NewGuid().ToString();

                return _guidId;
            }
            set
            {
                _fixedId = value;
            }
        }

        public string entity;
        public string state;
        public Direction direction;
        public RespawnBehavior respawn;
        public int screenX;
        public int screenY;

        public override bool Equals(object obj)
        {
            if (obj is EntityPlacement)
                return ((EntityPlacement)obj).Id == this.Id;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public EntityPlacement Clone()
        {
            return new EntityPlacement() {
                entity = this.entity,
                direction = this.direction,
                state = this.state,
                respawn = this.respawn,
                screenX = this.screenX,
                screenY = this.screenY
            };
        }
    }
}
