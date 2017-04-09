using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class SetVarEffectPartInfo : IEffectPartInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public IEffectPartInfo Clone()
        {
            return new SetVarEffectPartInfo() {
                Name = this.Name,
                Value = this.Value
            };
        }
    }
}
