namespace MegaMan.Common.Entities.Effects
{
    public class LadderEffectPartInfo : IEffectPartInfo
    {
        public LadderAction Action { get; set; }

        public IEffectPartInfo Clone()
        {
            return new LadderEffectPartInfo {
                Action = Action
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
