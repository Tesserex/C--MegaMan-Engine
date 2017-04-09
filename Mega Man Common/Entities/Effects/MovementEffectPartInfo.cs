using System;

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
            return new MovementEffectPartInfo() {
                Floating = this.Floating,
                FlipSprite = this.FlipSprite,
                X = this.X.Clone(),
                Y = this.Y.Clone(),
                Both = this.Both.Clone()
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
            return new VelocityEffectInfo() {
                Direction = this.Direction,
                Magnitude = this.Magnitude,
                MagnitudeVarName = this.MagnitudeVarName
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
