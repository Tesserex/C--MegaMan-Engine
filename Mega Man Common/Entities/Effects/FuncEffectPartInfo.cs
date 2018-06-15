using System.Collections.Generic;

namespace MegaMan.Common.Entities.Effects
{
    public class FuncEffectPartInfo : IEffectPartInfo
    {
        public IEnumerable<string> Statements { get; set; }

        public IEffectPartInfo Clone()
        {
            return new FuncEffectPartInfo {
                Statements = new List<string>(Statements)
            };
        }
    }
}
