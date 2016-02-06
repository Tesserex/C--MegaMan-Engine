using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            else
                return entity =>
                {
                    entity.GetComponent<InputComponent>().Paused = false;
                };
        }
    }
}
