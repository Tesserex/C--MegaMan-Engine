namespace MegaMan.Common.Entities.Effects
{
    public class PauseEffectPartInfo : IEffectPartInfo
    {
        public IEffectPartInfo Clone()
        {
            return new PauseEffectPartInfo();
        }
    }
}
