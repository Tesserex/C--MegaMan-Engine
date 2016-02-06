using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class FuncEffectPartInfo : IEffectPartInfo
    {
        public IEnumerable<string> Statements { get; set; }
    }
}
