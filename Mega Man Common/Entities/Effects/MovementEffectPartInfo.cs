namespace MegaMan.Common.Entities.Effects
{
    public class MovementEffectPartInfo : IEffectPartInfo
    {
        public bool? Floating { get; set; }
        public bool? FlipSprite { get; set; }
        public VelocityEffectInfo X { get; set; }
        public VelocityEffectInfo Y { get; set; }
        public VelocityEffectInfo Both { get; set; }
    }

    public class VelocityEffectInfo
    {
        public MovementEffectDirection Direction { get; set; }
        public float? Magnitude { get; set; }
        public string MagnitudeVarName { get; set; }
    }

    public enum MovementEffectDirection
    {
        Up,
        Down,
        Left,
        Right,
        Same,
        Reverse,
        Inherit,
        Input,
        Player
    }
}
