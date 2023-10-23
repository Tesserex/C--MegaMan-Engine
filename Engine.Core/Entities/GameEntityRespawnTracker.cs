using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;

namespace MegaMan.Engine.Entities
{
    public class GameEntityRespawnTracker : IEntityRespawnTracker
    {
        private readonly Dictionary<EntityPlacement, bool> _respawnableEntities = new Dictionary<EntityPlacement, bool>();

        public void Track(EntityPlacement placement, GameEntity entity)
        {
            entity.Removed += () => DisableRespawn(placement, entity);
        }

        private void DisableRespawn(EntityPlacement placement, GameEntity entity)
        {
            if (placement.Respawn != RespawnBehavior.Offscreen)
                _respawnableEntities[placement] = false;

            entity.Removed -= () => DisableRespawn(placement, entity);
        }

        public void ResetStage()
        {
            foreach (var placement in _respawnableEntities.Keys.Where(p => p.Respawn == RespawnBehavior.Stage))
            {
                _respawnableEntities[placement] = true;
            }
        }

        public void ResetDeath()
        {
            foreach (var placement in _respawnableEntities.Keys.Where(p => p.Respawn == RespawnBehavior.Death))
            {
                _respawnableEntities[placement] = true;
            }
        }

        public bool IsRespawnable(EntityPlacement placement)
        {
            if (_respawnableEntities.ContainsKey(placement))
                return _respawnableEntities[placement];

            return true;
        }
    }
}
