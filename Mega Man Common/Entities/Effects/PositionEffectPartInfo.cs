using System;

namespace MegaMan.Common.Entities.Effects
{
    public class PositionEffectPartInfo : IEffectPartInfo
    {
        public PositionEffectAxisInfo X { get; set; }
        public PositionEffectAxisInfo Y { get; set; }

        public IEffectPartInfo Clone()
        {
            return new PositionEffectPartInfo() {
                X = this.X.Clone(),
                Y = this.Y.Clone()
            };
        }
    }

    public class PositionEffectAxisInfo
    {
        public float? Base { get; set; }
        public float? Offset { get; set; }
        public string BaseVar { get; set; }
        public string OffsetVar { get; set; }
        public OffsetDirection OffsetDirection { get; set; }

        public PositionEffectAxisInfo Clone()
        {
            return new PositionEffectAxisInfo() {
                Base = this.Base,
                Offset = this.Offset,
                BaseVar = this.BaseVar,
                OffsetDirection = this.OffsetDirection,
                OffsetVar = this.OffsetVar
            };
        }
    }

    public enum OffsetDirection
    {
        Inherit,
        Input,
        Up,
        Down,
        Left,
        Right
    }
}
