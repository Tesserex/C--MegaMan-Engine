using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Entities
{
    public interface IEntityPool
    {
        GameEntity CreateEntity(string name);
        GameEntity CreateEntityWithId(string id, string name);
        GameEntity GetEntityById(string id);
        Int32 GetNumberAlive(string name);
        Int32 GetTotalAlive();
        IEnumerable<GameEntity> GetAll();
        void RemoveAll();
    }
}
