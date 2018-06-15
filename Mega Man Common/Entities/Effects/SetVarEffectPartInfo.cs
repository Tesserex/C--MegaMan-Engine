namespace MegaMan.Common.Entities.Effects
{
    public class SetVarEffectPartInfo : IEffectPartInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public IEffectPartInfo Clone()
        {
            return new SetVarEffectPartInfo {
                Name = Name,
                Value = Value
            };
        }
    }
}
