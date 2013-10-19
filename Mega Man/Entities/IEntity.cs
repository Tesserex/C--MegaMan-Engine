using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Entities
{
    public interface IEntity
    {
        string Name { get; }
        IGameplayContainer Container { get; }
        ITiledScreen Screen { get; }
        IEntityPool Entities { get; }
        bool IsGravitySensitive { get; }
        bool Paused { get; set; }
        IEntity Parent { get; }
        Direction Direction { get; }

        T GetComponent<T>() where T : Component;
        Component GetOrCreateComponent(string name);
        GameEntity Spawn(string entityName);
        void Remove();
        void Die();
        void SendMessage(IGameMessage message);
    }
}
