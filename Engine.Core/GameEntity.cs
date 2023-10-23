using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MegaMan.Common;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    [DebuggerDisplay("{Name}, Parent = {Parent!=null? Parent.Name : null}")]
    public class GameEntity : IEntity
    {
        private readonly Dictionary<Type, Component> components;
        private IGameplayContainer container;

        public string Name { get; set; }
        public IGameplayContainer Container { get { return container; } }
        public ITiledScreen Screen { get { return container.Screen; } }
        public IEntityPool Entities { get { return container.Entities; } }
        public IEntity Parent { get; private set; }
        public bool Running { get; private set; }
        public int MaxAlive { get; set; }
        public bool IsGravitySensitive { get; set; }   // whether to react to gravity flipping (collision and sprite)
        public bool Paused { get; set; }

        // I know this defeats good component based design but its just so much easier
        public Direction Direction
        {
            get
            {
                var movement = GetComponent<MovementComponent>();
                if (movement != null) return movement.Direction;
                return Direction.Right;
            }
            set
            {
                var movement = GetComponent<MovementComponent>();
                if (movement != null) movement.Direction = value;
            }
        }

        // there are three levels of deletion. Each one fires the previous events.
        // Stopped is used internally. Removed is used for anything forcibly removed
        // by the game xml, and Death is used for actual enemy kills,
        // with effects and explosions and such.

        public Effect OnDeath = entity => { };
        public event Action Started;
        public event Action Stopped;
        public event Action Removed;
        public event Action Death;

        public GameEntity()
        {
            components = new Dictionary<Type, Component>();
            MaxAlive = 50;
        }

        public IEnumerable<Component> Components
        {
            get
            {
                return components.Values.ToList();
            }
        }

        public T GetComponent<T>() where T : Component
        {
            if (components.ContainsKey(typeof(T))) return (T)components[typeof(T)];
            return null;
        }

        public void Start(IGameplayContainer container)
        {
            this.container = container;

            foreach (var c in Components)
                c.Start(container);

            Started?.Invoke();

            Running = true;
        }

        public void Stop()
        {
            if (!Running) return;

            foreach (var c in Components)
                c.Stop(container);

            Stopped?.Invoke();
            Running = false;
        }

        public void Remove()
        {
            Removed?.Invoke();
            Stop();
        }

        public void Die()
        {
            OnDeath(this);
            Death?.Invoke();
            Remove();
        }

        public void AddComponent(Component component)
        {
            if (components.ContainsKey(component.GetType())) return;

            component.Parent = this;
            foreach (var c in Components)
            {
                c.RegisterDependencies(component);
                component.RegisterDependencies(c);
            }
            components.Add(component.GetType(), component);
        }

        public void SendMessage(IGameMessage message)
        {
            foreach (var c in Components)
            {
                c.Message(message);
            }
        }

        public GameEntity Spawn(string entityName)
        {
            var spawn = Entities.CreateEntity(entityName);
            if (spawn != null)
            {
                spawn.Parent = this;
                spawn.Start(container);
            }

            return spawn;
        }

        public void CreateComponentIfNotExists<T>() where T : Component, new()
        {
            if (!components.ContainsKey(typeof(T)))
            {
                var comp = new T();
                AddComponent(comp);
            }
        }

        // this is for the XML to use
        public static int NumAlive(string name)
        {
            return Game.XmlNumAlive(name);
        }
    }
}
