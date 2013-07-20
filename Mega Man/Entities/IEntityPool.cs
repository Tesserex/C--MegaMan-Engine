using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Entities
{
    public interface IEntityPool
    {
        GameEntity CreateEntity(string name);
        Int32 GetNumberAlive(string name);
        Int32 GetTotalAlive();
        IEnumerable<GameEntity> GetAll();
    }
}
