namespace MegaMan.Common.Entities.Effects
{
    public class PositionEffectPartInfo : IEffectPartInfo
    {
        public PositionEffectAxisInfo X { get; set; }
        public PositionEffectAxisInfo Y { get; set; }

        public IEffectPartInfo Clone()
        {
            return new PositionEffectPartInfo {
                X = X.Clone(),
                Y = Y.Clone()
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
            return new PositionEffectAxisInfo {
                Base = Base,
                Offset = Offset,
                BaseVar = BaseVar,
                OffsetDirection = OffsetDirection,
                OffsetVar = OffsetVar
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
