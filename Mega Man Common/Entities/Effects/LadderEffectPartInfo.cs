using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class LadderEffectPartInfo : IEffectPartInfo
    {
        public LadderAction Action { get; set; }

        public IEffectPartInfo Clone()
        {
            return new LadderEffectPartInfo() {
                Action = this.Action
            };
        }
    }

    public enum LadderAction
    {
        Grab,
        LetGo,
        StandOn,
        ClimbDown
    }
}
