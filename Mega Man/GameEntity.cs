using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using MegaMan.Common;

namespace MegaMan.Engine
{
    [DebuggerDisplay("{Name}, Parent = {Parent!=null? Parent.Name : null}, {numAlive} Alive")]
    public class GameEntity
    {
        private readonly Dictionary<Type, Component> components;
        public IGameplayContainer Container { get; private set; }
        public string Name { get; private set; }
        public IEntityContainer Screen { get { return Container.Entities; } }
        public GameEntity Parent { get; private set; }

        private bool running;

        private int maxAlive = 50;
        private int numAlive;
        public bool GravityFlip { get; private set; }   // whether to react to gravity flipping (collision and sprite)
        public bool Paused { get; set; }

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

        private Effect OnDeath = entity => { };
        public event Action Stopped;
        public event Action Removed;
        public event Action Death;

        private GameEntity(IGameplayContainer container = null)
        {
            components = new Dictionary<Type, Component>();
            this.Container = container;
        }

        public T GetComponent<T>() where T : Component
        {
            if (components.ContainsKey(typeof(T))) return (T)components[typeof(T)];
            return null;
        }

        public void Start()
        {
            if (entities[Name].numAlive >= entities[Name].maxAlive) return;
            entities[Name].numAlive++;
            foreach (Component c in components.Values) c.Start();
            RegisterEntity(this);
            running = true;
        }

        public void Stop() { Stop(true); }

        // there are three levels of deletion. Each one fires the previous events.
        // Stopped is used internally. Removed is used for anything forcibly removed
        // by the game xml, and Death is used for actual enemy kills,
        // with effects and explosions and such.

        private void Stop(bool remove)
        {
            if (!running) return;

            entities[Name].numAlive--;
            foreach (Component c in components.Values) c.Stop();
            if (Stopped != null) Stopped();
            if (remove) RemoveEntity(this);
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

        private void AddComponent(Component component)
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
            GameEntity spawn = Get(entityName, Container);
            if (spawn != null)
            {
                spawn.Parent = this;
                spawn.Start();
                Screen.AddEntity(spawn);
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

        private static readonly Dictionary<string, GameEntity> entities = new Dictionary<string,GameEntity>();
        private static readonly Dictionary<string, TileProperties> entityProperties = new Dictionary<string, TileProperties>();

        static GameEntity()
        {
            entityProperties["Default"] = TileProperties.Default;
        }

        public static void LoadEntities(XElement doc)
        {
            // properties
            XElement propHead = doc.Element("Properties");
            if (propHead != null)
            {
                foreach (XElement propNode in propHead.Elements("Properties"))
                {
                    TileProperties p = new TileProperties(propNode);
                    entityProperties[p.Name] = p;
                }
            }

            foreach (XElement entity in doc.Elements("Entity"))
            {
                LoadEntity(entity);
            }
        }

        private static void LoadEntity(XElement xml)
        {
            GameEntity entity = new GameEntity();
            string name = xml.RequireAttribute("name").Value;

            if (entities.ContainsKey(name)) throw new GameXmlException(xml, "You have defined two entities both named \"" + name + "\".");

            entity.Name = name;

            int limit;
            if (xml.TryInteger("limit", out limit)) entity.maxAlive = limit;

            SpriteComponent spritecomp = null;
            PositionComponent poscomp = null;
            StateComponent statecomp = new StateComponent();
            entity.AddComponent(statecomp);

            try
            {
                foreach (XElement xmlComp in xml.Elements())
                {
                    switch (xmlComp.Name.LocalName)
                    {
                        case "Tilesheet":
                            if (spritecomp == null)
                            {
                                spritecomp = new SpriteComponent();
                                entity.AddComponent(spritecomp);
                            }
                            if (poscomp == null)
                            {
                                poscomp = new PositionComponent();
                                entity.AddComponent(poscomp);
                            }
                            spritecomp.LoadTilesheet(xmlComp);
                            break;

                        case "Trigger":
                            statecomp.LoadStateTrigger(xmlComp);
                            break;

                        case "Sprite":
                            if (spritecomp == null)
                            {
                                spritecomp = new SpriteComponent();
                                entity.AddComponent(spritecomp);
                            }
                            if (poscomp == null)
                            {
                                poscomp = new PositionComponent();
                                entity.AddComponent(poscomp);
                            }
                            spritecomp.LoadXml(xmlComp);
                            break;

                        case "Position":
                            if (poscomp == null)
                            {
                                poscomp = new PositionComponent();
                                entity.AddComponent(poscomp);
                            }
                            poscomp.LoadXml(xmlComp);
                            break;

                        case "Death":
                            entity.OnDeath += EffectParser.LoadTriggerEffect(xmlComp);
                            break;

                        case "GravityFlip":
                            entity.GravityFlip = xmlComp.GetBool();
                            break;

                        default:
                            entity.GetOrCreateComponent(xmlComp.Name.LocalName).LoadXml(xmlComp);
                            break;
                    }
                }
            }
            catch (GameXmlException ex)
            {
                ex.Entity = name;
                throw;
            }

            entities.Add(name, entity);
        }

        public static int NumAlive(string name)
        {
            return entities[name].numAlive;
        }

        public static GameEntity Get(string name, IGameplayContainer container)
        {
            if (!entities.ContainsKey(name)) throw new GameRunException("Someone requested an entity named \"" + name + "\", but I couldn't find it!\n" +
                "You need to make sure it's defined in one of the included XML files.");

            // clone it
            GameEntity entity = new GameEntity(container);
            GameEntity source = entities[name];

            if (source.numAlive >= source.maxAlive) return null;

            foreach (Component c in source.components.Values)
            {
                entity.AddComponent(c.Clone());
            }
            entity.Name = source.Name;
            entity.OnDeath = source.OnDeath;
            entity.GravityFlip = source.GravityFlip;
            return entity;
        }

        public static TileProperties GetProperties(string name)
        {
            if (entityProperties.ContainsKey(name)) return entityProperties[name];
            return TileProperties.Default;
        }

        private static readonly List<GameEntity> actives = new List<GameEntity>();

        public static int ActiveCount { get { return actives.Count; } }

        private static void RegisterEntity(GameEntity entity)
        {
            actives.Add(entity);
        }

        private static void RemoveEntity(GameEntity entity)
        {
            actives.Remove(entity);
        }

        public static IEnumerable<GameEntity> GetAll()
        {
            return actives;
        }

        public static void StopAll()
        {
            foreach (GameEntity entity in actives) entity.Stop(false);
            actives.Clear();
        }

        public static void UnloadAll()
        {
            // stop all the real ones in play
            StopAll();

            // now destroy the originals
            foreach (GameEntity entity in entities.Values)
            {
                entity.Stop(false);
            }
            entities.Clear();

            EffectParser.Unload();
        }
    }
}
