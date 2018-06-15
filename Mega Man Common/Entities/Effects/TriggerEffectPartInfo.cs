namespace MegaMan.Common.Entities.Effects
{
    public class TriggerEffectPartInfo : IEffectPartInfo
    {
        public TriggerInfo Trigger { get; set; }

        public IEffectPartInfo Clone()
        {
            return new TriggerEffectPartInfo {
                Trigger = Trigger.Clone()
            };
        }
    }
}
