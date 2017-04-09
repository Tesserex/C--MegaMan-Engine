using System;

namespace MegaMan.Common.Entities.Effects
{
    public class HealthEffectPartInfo : IEffectPartInfo
    {
        public float Change { get; set; }

        public IEffectPartInfo Clone()
        {
            return new HealthEffectPartInfo() {
                Change = this.Change
            };
        }
    }
}
