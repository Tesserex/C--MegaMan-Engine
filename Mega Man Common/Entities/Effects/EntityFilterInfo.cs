namespace MegaMan.Common.Entities.Effects
{
    public class EntityFilterInfo
    {
        public string Type { get; set; }
        public Direction? Direction { get; set; }
        public PositionFilter Position { get; set; }
    }
    
    public class PositionFilter
    {
        public RangeFilter X { get; set; }
        public RangeFilter Y { get; set; }
    }

    public class RangeFilter
    {
        public float? Min { get; set; }
        public float? Max { get; set; }
    }
}
