namespace MegaMan.Common.Entities.Effects
{
    public class RemoveEffectPartInfo : IEffectPartInfo
    {
        public IEffectPartInfo Clone()
        {
            return new RemoveEffectPartInfo();
        }
    }
}
