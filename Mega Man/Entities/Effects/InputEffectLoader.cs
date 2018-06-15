using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class InputEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(InputEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var input = (InputEffectPartInfo)info;

            if (input.Paused)
                return entity =>
                {
                    entity.GetComponent<InputComponent>().Paused = true;
                };
            return entity =>
            {
                entity.GetComponent<InputComponent>().Paused = false;
            };
        }
    }
}
