using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class NextEffectPartInfo : IEffectPartInfo
    {
        public HandlerTransfer Transfer { get; set; }

        public IEffectPartInfo Clone()
        {
            return new NextEffectPartInfo() {
                Transfer = this.Transfer.Clone()
            };
        }
    }
}
