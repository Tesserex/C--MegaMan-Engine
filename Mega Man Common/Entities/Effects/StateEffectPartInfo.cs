namespace MegaMan.Common.Entities.Effects
{
    public class StateEffectPartInfo : IEffectPartInfo
    {
        public string Name { get; set; }

        public IEffectPartInfo Clone()
        {
            return new StateEffectPartInfo {
                Name = Name
            };
        }
    }
}
