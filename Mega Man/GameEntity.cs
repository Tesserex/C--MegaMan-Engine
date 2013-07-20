using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using MegaMan.Common;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    [DebuggerDisplay("{Name}, Parent = {Parent!=null? Parent.Name : null}")]
    public class GameEntity
    {
        private readonly Dictionary<Type, Component> components;
        private IGameplayContainer container;

        public string Name { get; set; }
        public IEntityContainer Screen { get { return container.Entities; } }
        public GameEntity Parent { get; private set; }

        private bool running;

        public int MaxAlive { get; set; }
        public bool GravityFlip { get; set; }   // whether to react to gravity flipping (collision and sprite)
        public bool Paused { get; set; }

        public IEntityPool EntityPool { get; private set; }

        // I know this defeats good component based design but its just so much easier
        public Direction Direction
        {
            get
            {
                MovementComponent movement = GetComponent<MovementComponent>();
                if (movement != null) return movement.Direction;
                return Direction.Right;
            }
            set
            {
                MovementComponent movement = GetComponent<MovementComponent>();
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

        public GameEntity(IEntityPool entityPool = null)
        {
            components = new Dictionary<Type, Component>();
            MaxAlive = 50;
            EntityPool = entityPool;
        }

        public IEnumerable<Component> Components
        {
            get
            {
                return components.Values;
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

            foreach (Component c in components.Values)
                c.Start(container);

            if (Started != null)
                Started();

            running = true;
        }

        public void Stop()
        {
            if (!running) return;

            foreach (Component c in components.Values) c.Stop(container);
            if (Stopped != null) Stopped();
            running = false;
        }

        public void Remove()
        {
            if (Removed != null) Removed();
            Stop();
        }

        public void Die()
        {
            OnDeath(this);
            if (Death != null) Death();
            Remove();
        }

        public void AddComponent(Component component)
        {
            if (components.ContainsKey(component.GetType())) return;

            component.Parent = this;
            foreach (Component c in components.Values)
            {
                c.RegisterDependencies(component);
                component.RegisterDependencies(c);
            }
            components.Add(component.GetType(), component);
        }

        public void SendMessage(IGameMessage message)
        {
            foreach (Component c in components.Values)
            {
                c.Message(message);
            }
        }

        public GameEntity Spawn(string entityName)
        {
            GameEntity spawn = EntityPool.CreateEntity(entityName);
            if (spawn != null)
            {
                spawn.Parent = this;
                spawn.Start(container);
                Screen.AddEntity(Guid.NewGuid().ToString(), spawn);
            }

            return spawn;
        }

        public Component GetOrCreateComponent(string name)
        {
            // handle plural cases
            if (name == "Sounds") name = "Sound";
            if (name == "Weapons") name = "Weapon";

            string typename = name + "Component";
            Type comptype = Type.GetType("MegaMan.Engine." + typename, false, true);
            if (comptype == null) return null;
            Component comp;
            if (components.ContainsKey(comptype)) comp = components[comptype];
            else // create one
            {
                comp = (Component)Activator.CreateInstance(comptype);
                AddComponent(comp);
            }
            return comp;
        }

        public static Effect ParseComponentEffect(XElement effectNode)
        {
            Type componentType = Type.GetType("MegaMan.Engine." + effectNode.Name.LocalName + "Component");
            if (componentType == null) throw new GameXmlException(effectNode, String.Format("Expected a component name, but {0} is not a component!", effectNode.Name.LocalName));
            var method = componentType.GetMethod("ParseEffect");
            return (Effect)method.Invoke(null, new[] {effectNode});
        }
    }
}
