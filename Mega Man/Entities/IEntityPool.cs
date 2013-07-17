using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Entities
{
    interface IEntityPool
    {
        GameEntity CreateEntity(string name, IGameplayContainer container);
        Int32 GetNumberAlive(string name);
        Int32 GetTotalAlive();
    }
}
