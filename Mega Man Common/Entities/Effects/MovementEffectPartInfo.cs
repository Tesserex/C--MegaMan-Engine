namespace MegaMan.Common.Entities.Effects
{
    public class MovementEffectPartInfo : IEffectPartInfo
    {
        public bool? Floating { get; set; }
        public bool? FlipSprite { get; set; }
        public VelocityEffectInfo X { get; set; }
        public VelocityEffectInfo Y { get; set; }
        public VelocityEffectInfo Both { get; set; }

        public IEffectPartInfo Clone()
        {
            return new MovementEffectPartInfo {
                Floating = Floating,
                FlipSprite = FlipSprite,
                X = X.Clone(),
                Y = Y.Clone(),
                Both = Both.Clone()
            };
        }
    }

    public class VelocityEffectInfo
    {
        public MovementEffectDirection Direction { get; set; }
        public float? Magnitude { get; set; }
        public string MagnitudeVarName { get; set; }

        public VelocityEffectInfo Clone()
        {
            return new VelocityEffectInfo {
                Direction = Direction,
                Magnitude = Magnitude,
                MagnitudeVarName = MagnitudeVarName
            };
        }
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
