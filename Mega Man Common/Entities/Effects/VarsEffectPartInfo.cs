namespace MegaMan.Common.Entities.Effects
{
    public class VarsEffectPartInfo : IEffectPartInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Call { get; set; }
        public string EntityName { get; set; }

        public IEffectPartInfo Clone()
        {
            return new VarsEffectPartInfo {
                Name = Name,
                Value = Value,
                Call = Call,
                EntityName = EntityName
            };
        }
    }
}
