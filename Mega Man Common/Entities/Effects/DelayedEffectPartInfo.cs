namespace MegaMan.Common.Entities.Effects
{
    public class DelayedEffectPartInfo : IEffectPartInfo
    {
        public int DelayFrames { get; set; }
        public EffectInfo Effect { get; set; }

        public IEffectPartInfo Clone()
        {
            return new DelayedEffectPartInfo {
                DelayFrames = DelayFrames,
                Effect = Effect.Clone()
            };
        }
    }
}
