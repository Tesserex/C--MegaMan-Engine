namespace MegaMan.Common.Entities.Effects
{
    public class InputEffectPartInfo : IEffectPartInfo
    {
        public bool Paused { get; set; }

        public IEffectPartInfo Clone()
        {
            return new InputEffectPartInfo {
                Paused = Paused
            };
        }
    }
}
