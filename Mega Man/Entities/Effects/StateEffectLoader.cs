using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class StateEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(StateEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var newstate = ((StateEffectPartInfo)info).Name;
            return entity => {
                StateComponent state = entity.GetComponent<StateComponent>();
                if (state != null)
                {
                    state.ChangeState(newstate);
                }
            };
        }
    }
}
