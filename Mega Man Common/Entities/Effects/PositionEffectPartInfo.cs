namespace MegaMan.Common.Entities.Effects
{
    public class PositionEffectPartInfo : IEffectPartInfo
    {
        public PositionEffectAxisInfo X { get; set; }
        public PositionEffectAxisInfo Y { get; set; }
    }

    public class PositionEffectAxisInfo
    {
        public float? Base { get; set; }
        public float? Offset { get; set; }
        public string BaseVar { get; set; }
        public string OffsetVar { get; set; }
        public OffsetDirection OffsetDirection { get; set; }
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
