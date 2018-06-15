namespace MegaMan.Common.Entities.Effects
{
    public class NextEffectPartInfo : IEffectPartInfo
    {
        public HandlerTransfer Transfer { get; set; }

        public IEffectPartInfo Clone()
        {
            return new NextEffectPartInfo {
                Transfer = Transfer.Clone()
            };
        }
    }
}
