using System.Collections.Generic;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.Common.Entities.Effects
{
    public class EffectInfo : IncludedObject
    {
        public string Name { get; set; }
        public IEnumerable<IEffectPartInfo> Parts { get; set; }
    }
}
