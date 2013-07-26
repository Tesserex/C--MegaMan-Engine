using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
