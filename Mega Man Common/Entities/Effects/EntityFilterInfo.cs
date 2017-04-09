using System;

namespace MegaMan.Common.Entities.Effects
{
    public class EntityFilterInfo
    {
        public string Type { get; set; }
        public Direction? Direction { get; set; }
        public PositionFilter Position { get; set; }

        public EntityFilterInfo Clone()
        {
            return new EntityFilterInfo() {
                Type = this.Type,
                Direction = this.Direction,
                Position = this.Position.Clone()
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
