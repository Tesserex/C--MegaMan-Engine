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
            return new VarsEffectPartInfo() {
                Name = this.Name,
                Value = this.Value,
                Call = this.Call,
                EntityName = this.EntityName
            };
        }
    }
}
