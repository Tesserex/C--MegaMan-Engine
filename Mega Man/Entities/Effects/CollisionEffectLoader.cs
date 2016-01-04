using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class CollisionEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(CollisionEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var colInfo = (CollisionEffectPartInfo)info;

            Effect effect = entity => {};

            if (colInfo.Enabled.HasValue)
            {
                var enable = colInfo.Enabled.Value;
                effect += entity => {
                    CollisionComponent col = entity.GetComponent<CollisionComponent>();
                    if (col != null) col.Enabled = enable;
                };
            }

            if (colInfo.HitBoxes.Any() || colInfo.EnabledBoxes.Any() || colInfo.ClearEnabled)
            {
                var collisionBoxes = colInfo.HitBoxes.Select(b => new CollisionBox(b));
                effect += entity => {
                    HitBoxMessage msg = new HitBoxMessage(entity, collisionBoxes, colInfo.EnabledBoxes, colInfo.ClearEnabled);
                    entity.SendMessage(msg);
                };
            }

            return effect;
        }
    }
}
