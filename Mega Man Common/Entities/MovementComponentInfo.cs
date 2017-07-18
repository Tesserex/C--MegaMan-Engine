using MegaMan.Common.Entities.Effects;

namespace MegaMan.Common.Entities
{
    public class MovementComponentInfo : IComponentInfo
    {
        public MovementEffectPartInfo EffectInfo { get; set; }

        public MovementComponentInfo()
        {
            EffectInfo = new MovementEffectPartInfo();
        }
    }
}
