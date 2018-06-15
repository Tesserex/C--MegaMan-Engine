using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class SpawnEffectLoader : IEffectLoader
    {
        private readonly PositionEffectLoader _posLoader;

        public SpawnEffectLoader(PositionEffectLoader posLoader)
        {
            _posLoader = posLoader;
        }

        public Type PartInfoType
        {
            get
            {
                return typeof(SpawnEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var spawnInfo = (SpawnEffectPartInfo)info;

            Effect posEff = null;
            if (spawnInfo.Position != null)
            {
                posEff = _posLoader.Load(spawnInfo.Position);
            }

            return entity => {
                var spawn = entity.Spawn(spawnInfo.Name);
                if (spawn == null) return;
                var msg = new StateMessage(entity, spawnInfo.State);
                spawn.SendMessage(msg);
                posEff?.Invoke(spawn);
            };
        }
    }
}
