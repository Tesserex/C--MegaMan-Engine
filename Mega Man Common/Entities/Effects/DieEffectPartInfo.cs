namespace MegaMan.Common.Entities.Effects
{
    public class DieEffectPartInfo : IEffectPartInfo
    {
        public IEffectPartInfo Clone()
        {
            return new DieEffectPartInfo();
        }
    }
}
