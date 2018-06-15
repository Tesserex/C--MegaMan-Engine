using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class LadderEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(LadderEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var ladderInfo = (LadderEffectPartInfo)info;

            Effect effect = e => { };

            if (ladderInfo.Action == LadderAction.Grab)
                effect = entity =>
                {
                    var ladder = entity.GetComponent<LadderComponent>();
                    if (ladder != null) ladder.Grab();
                };
            else if (ladderInfo.Action == LadderAction.LetGo)
                effect = entity =>
                {
                    var ladder = entity.GetComponent<LadderComponent>();
                    if (ladder != null) ladder.LetGo();
                };
            else if (ladderInfo.Action == LadderAction.StandOn)
                effect = entity =>
                {
                    var ladder = entity.GetComponent<LadderComponent>();
                    if (ladder != null) ladder.StandOn();
                };
            else if (ladderInfo.Action == LadderAction.ClimbDown)
                effect = entity =>
                {
                    var ladder = entity.GetComponent<LadderComponent>();
                    if (ladder != null) ladder.ClimbDown();
                };

            return effect;
        }
    }
}
