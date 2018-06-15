using System.Collections.Generic;
using System.Linq;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.Common.Entities.Effects
{
    public class EffectInfo : IncludedObject
    {
        public string Name { get; set; }
        public EntityFilterInfo Filter { get; set; }
        public IEnumerable<IEffectPartInfo> Parts { get; set; }

        public EffectInfo Clone()
        {
            return new EffectInfo {
                Name = Name,
                Filter = Filter.Clone(),
                Parts = Parts.Select(x => x.Clone()).ToList()
            };
        }
    }
}
