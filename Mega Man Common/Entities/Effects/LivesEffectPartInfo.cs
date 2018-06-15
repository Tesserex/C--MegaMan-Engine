namespace MegaMan.Common.Entities.Effects
{
    public class LivesEffectPartInfo : IEffectPartInfo
    {
        public int Add { get; set; }

        public IEffectPartInfo Clone()
        {
            return new LivesEffectPartInfo {
                Add = Add
            };
        }
    }
}
