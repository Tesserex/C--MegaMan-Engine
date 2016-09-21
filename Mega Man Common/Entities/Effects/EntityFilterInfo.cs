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
    }

    public class PositionFilter
    {
        public RangeFilter X { get; set; }
        public RangeFilter Y { get; set; }
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
    }
}
