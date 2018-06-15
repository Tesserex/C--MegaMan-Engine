namespace MegaMan.Common.Entities.Effects
{
    public class UnpauseEffectPartInfo : IEffectPartInfo
    {
        public IEffectPartInfo Clone()
        {
            return new UnpauseEffectPartInfo();
        }
    }
}
