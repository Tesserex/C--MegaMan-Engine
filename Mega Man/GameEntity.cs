using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;

namespace Mega_Man
{
    [DebuggerDisplay("{Name}, Parent = {Parent!=null? Parent.Name : null}, {numAlive} Alive")]
    public class GameEntity
    {
        private Dictionary<Type, Component> components;
        public string Name { get; set; }
        public ScreenHandler Screen { get; set; }
        public GameEntity Parent { get; private set; }

        private bool running;

        private int maxAlive = 50;
        private int numAlive;
        public bool GravityFlip { get; private set; }   // whether to react to gravity flipping (collision and sprite)
        public bool Paused { get; set; }

        // I know this defeats good component based design but its just so much easier
        private Direction dir;
        public Direction Direction
        {
            get
            {
                MovementComponent movement = GetComponent<MovementComponent>();
                if (movement != null) return movement.Direction;
                return dir;
            }
            set { dir = value; }
        }

        private Effect OnDeath = (entity) => { };
        public event Action Stopped;
        public event Action Death;

        public GameEntity()
        {
            components = new Dictionary<Type, Component>();
        }

        public GameEntity(GameEntity parent) : this()
        {
            Parent = parent;
        }

        public T GetComponent<T>() where T : Component
        {
            if (components.ContainsKey(typeof(T))) return (T)components[typeof(T)];
            return null;
        }

        public void Start()
        {
            if (entities[this.Name].numAlive >= entities[this.Name].maxAlive) return;
            entities[this.Name].numAlive++;
            Screen = Game.CurrentGame.CurrentMap.CurrentScreen;
            foreach (Component c in components.Values) c.Start();
            RegisterEntity(this);
            running = true;
        }

        public void Stop() { Stop(true); }

        private void Stop(bool remove)
        {
            if (!running) return;

            entities[this.Name].numAlive--;
            foreach (Component c in components.Values) c.Stop();
            if (Stopped != null) Stopped();
            if (remove) RemoveEntity(this);
            running = false;
        }

        public void Die()
        {
            Stop();
            OnDeath(this);
            if (Death != null) Death();
        }

        public void AddComponent(Component component)
        {
            if (components.ContainsKey(component.GetType())) return;

            component.Parent = this;
            foreach (Component c in this.components.Values)
            {
                c.RegisterDependencies(component);
                component.RegisterDependencies(c);
            }
            this.components.Add(component.GetType(), component);
        }

        public void SendMessage(IGameMessage message)
        {
            foreach (Component c in this.components.Values)
            {
                c.Message(message);
            }
        }

        public GameEntity Spawn(string entityName)
        {
            GameEntity spawn = Get(entityName);
            if (spawn != null)
            {
                spawn.Parent = this;
                spawn.Start();
            }
            return spawn;
        }

        private Component GetOrCreateComponent(string name)
        {
            // handle plural cases
            if (name == "Sounds") name = "Sound";
            if (name == "Weapons") name = "Weapon";

            string typename = name + "Component";
            Type comptype = Type.GetType("Mega_Man." + typename, false, true);
            if (comptype == null) return null;
            Component comp;
            if (components.ContainsKey(comptype)) comp = components[comptype];
            else // create one
            {
                comp = (Component)Activator.CreateInstance(comptype);
                this.AddComponent(comp);
            }
            return comp;
        }

        // unfortunately, you cannot have abstract static methods.
        // So parsing effects is an instance method, but doesn't
        // really need to be, except for the compiler to be happy.
        public Effect ParseComponentEffect(XElement effectNode)
        {
            Component comp = GetOrCreateComponent(effectNode.Name.LocalName);
            return comp.ParseEffect(effectNode);
        }

        private static Dictionary<string, GameEntity> entities = new Dictionary<string,GameEntity>();
        private static Dictionary<string, MegaMan.TileProperties> entityProperties = new Dictionary<string, MegaMan.TileProperties>();

        static GameEntity()
        {
            entityProperties["Default"] = MegaMan.TileProperties.Default;
        }

        public static void LoadEntities(string xmlPath)
        {
            XElement doc = XElement.Load(xmlPath, LoadOptions.SetLineInfo);

            // properties
            XElement propHead = doc.Element("Properties");
            if (propHead != null)
            {
                foreach (XElement propNode in propHead.Elements("Properties"))
                {
                    MegaMan.TileProperties p = new MegaMan.TileProperties(propNode);
                    entityProperties[p.Name] = p;
                }
            }

            foreach (XElement entity in doc.Elements("Entity"))
            {
                try
                {
                    LoadEntity(entity);
                }
                catch (EntityXmlException ex)
                {
                    ex.File = xmlPath;
                    throw;
                }
            }
        }

        public static void LoadEntity(XElement xml)
        {
            GameEntity entity = new GameEntity();
            string name = xml.Attribute("name").Value;

            entity.Name = name;

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
                            foreach (XElement xmlchild in xmlComp.Elements())
                            {
                                switch (xmlchild.Name.LocalName)
                                {
                                    case "Spawn":
                                        entity.OnDeath += LoadSpawnEffect(xmlchild);
                                        break;

                                    case "Trigger":
                                        string conditionString;
                                        if (xmlchild.Attribute("condition") != null) conditionString = xmlchild.Attribute("condition").Value;
                                        else conditionString = xmlchild.Element("Condition").Value;
                                        Condition condition = statecomp.ParseCondition(conditionString);
                                        Effect effect = statecomp.LoadTriggerEffect(xmlchild.Element("Effect"));
                                        entity.OnDeath += (e) =>
                                            {
                                                if (condition(e)) effect(e);
                                            };
                                        break;

                                    default:
                                        entity.OnDeath += entity.ParseComponentEffect(xmlchild);
                                        break;
                                }
                            }
                            break;

                        case "GravityFlip":
                            bool grav;
                            if (!bool.TryParse(xmlComp.Value, out grav)) throw new EntityXmlException(null, (xmlComp as System.Xml.IXmlLineInfo).LineNumber, entity.Name, "GravityFlip", null, "GravityFlip value must represent True or False");
                            entity.GravityFlip = grav;
                            break;

                        default:
                            entity.GetOrCreateComponent(xmlComp.Name.LocalName).LoadXml(xmlComp);
                            break;
                    }
                }
            }
            catch (EntityXmlException ex)
            {
                ex.Entity = name;
                throw;
            }

            entities.Add(name, entity);
        }

        public static Effect LoadSpawnEffect(XElement node)
        {
            string name = node.Attribute("name").Value;
            string statename = "Start";
            if (node.Attribute("state") != null) statename = node.Attribute("state").Value;
            XElement posNodeX = node.Element("X");
            XElement posNodeY = node.Element("Y");
            Effect posEff = null;
            if (posNodeX != null)
            {
                posEff = PositionComponent.ParsePositionBehavior(posNodeX, Axis.X);
            }
            if (posNodeY != null) posEff += PositionComponent.ParsePositionBehavior(posNodeY, Axis.Y);
            return (entity) =>
            {
                GameEntity spawn = entity.Spawn(name);
                if (spawn == null) return;
                StateMessage msg = new StateMessage(entity, statename);
                spawn.SendMessage(msg);
                if (posEff != null) posEff(spawn);
            };
        }

        public static GameEntity Get(string name)
        {
            // clone it
            GameEntity entity = new GameEntity();
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

        public static MegaMan.TileProperties GetProperties(string name)
        {
            if (entityProperties.ContainsKey(name)) return entityProperties[name];
            return MegaMan.TileProperties.Default;
        }

        private static List<GameEntity> actives = new List<GameEntity>();

        public static int ActiveCount { get { return actives.Count; } }

        public static void RegisterEntity(GameEntity entity)
        {
            actives.Add(entity);
        }

        public static void RemoveEntity(GameEntity entity)
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
        }
    }
}
