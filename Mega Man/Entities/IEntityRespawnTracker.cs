using MegaMan.Common;

namespace MegaMan.Engine.Entities
{
    public interface IEntityRespawnTracker
    {
        void Track(EntityPlacement placement, GameEntity entity);
        void ResetDeath();
        void ResetStage();
        bool IsRespawnable(EntityPlacement placement);
    }
}
