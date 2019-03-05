using System;

namespace MegaMan.Common.Entities.Effects
{
    public class EntityFilterInfo
    {
        public string Type { get; set; }
        public string State { get; set; }
        public Direction? Direction { get; set; }
        public PositionFilter Position { get; set; }
        public RangeFilter Health { get; set; }
        public MovementFilter Movement { get; set; }
        public CollisionFilter Collision { get; set; }

        public EntityFilterInfo Clone()
        {
            return new EntityFilterInfo() {
                Type = this.Type,
                Direction = this.Direction,
                Position = this.Position.Clone(),
                Health = this.Health.Clone(),
                Movement = this.Movement.Clone(),
                Collision = this.Collision.Clone()
            };
        }
    }

    public class PositionFilter
    {
        public RangeFilter X { get; set; }
        public RangeFilter Y { get; set; }

        public PositionFilter Clone()
        {
            return new PositionFilter() {
                X = this.X.Clone(),
                Y = this.Y.Clone()
            };
        }
    }

    public class MovementFilter
    {
        public RangeFilter X { get; set; }
        public RangeFilter Y { get; set; }
        public RangeFilter Total { get; set; }
    }

    public class CollisionFilter
    {
        public bool? BlockTop { get; set; }
        public bool? BlockBottom { get; set; }
        public bool? BlockLeft { get; set; }
        public bool? BlockRight { get; set; }
    }

    public class RangeFilter
    {
        public float? Min { get; set; }
        public float? Max { get; set; }

        public RangeFilter Clone()
        {
            return new RangeFilter() { Max = this.Max, Min = this.Min };
        }
    }
}
