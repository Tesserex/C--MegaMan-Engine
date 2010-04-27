using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mega_Man
{
    public class GameEntity
    {
        private Dictionary<Type, Component> components;
        public string Name { get; set; }
        public string Group { get; set; }
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
                MovementComponent movement = (MovementComponent)GetComponent(typeof(MovementComponent));
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

        public Component GetComponent(Type type)
        {
            if (components.ContainsKey(type)) return components[type];
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

            string group = "None";
            XAttribute groupattr = xml.Attribute("group");
            if (groupattr != null) group = groupattr.Value;

            entity.Name = name;
            entity.Group = group;

            SpriteComponent spritecomp = null;
            PositionComponent poscomp = null;
            StateComponent statecomp = new StateComponent();
            entity.AddComponent(statecomp);

            Dictionary<string, System.Drawing.Image> tilesheets = new Dictionary<string,System.Drawing.Image>();
            Dictionary<string, string> sheetpaths = new Dictionary<string, string>();

            try
            {
                foreach (XElement xmlComp in xml.Elements())
                {
                    switch (xmlComp.Name.LocalName)
                    {
                        case "Tilesheet":
                            XAttribute palAttr = xmlComp.Attribute("pallete");
                            string pallete = "Default";
                            if (palAttr != null) pallete = palAttr.Value;
                            string path = System.IO.Path.Combine(Game.CurrentGame.BasePath, xmlComp.Value);
                            System.Drawing.Bitmap sheet = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(path);
                            sheet.SetResolution(Const.Resolution, Const.Resolution);
                            if (!tilesheets.ContainsKey(pallete))
                            {
                                tilesheets.Add(pallete, sheet);
                                sheetpaths.Add(pallete, path);
                            }
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

                            string spriteName = "Default";
                            string spritePallete = "Default";
                            XAttribute spriteNameAttr = xmlComp.Attribute("name");
                            if (spriteNameAttr != null) spriteName = spriteNameAttr.Value;
                            XAttribute palleteAttr = xmlComp.Attribute("pallete");
                            if (palleteAttr != null) spritePallete = palleteAttr.Value;

                            if (xmlComp.Attribute("tilesheet") != null) // explicitly specified pallete for this sprite
                            {
                                MegaMan.Sprite spr = MegaMan.Sprite.FromXml(xmlComp, Game.CurrentGame.BasePath);
                                string sheetpath = System.IO.Path.Combine(Game.CurrentGame.BasePath, xmlComp.Attribute("tilesheet").Value);
                                spr.SetTexture(Engine.Instance.GraphicsDevice, sheetpath);
                                spritecomp.Add(spritePallete, spriteName, spr);
                            }
                            else // load sprite for all palletes
                            {
                                foreach (KeyValuePair<string, System.Drawing.Image> pair in tilesheets)
                                {
                                    MegaMan.Sprite sprite = MegaMan.Sprite.FromXml(xmlComp, pair.Value);
                                    sprite.SetTexture(Engine.Instance.GraphicsDevice, sheetpaths[pair.Key]);
                                    spritecomp.Add(pair.Key, spriteName, sprite);
                                }
                            }
                            break;

                        case "Position":
                            if (poscomp == null)
                            {
                                poscomp = new PositionComponent();
                                entity.AddComponent(poscomp);
                            }
                            poscomp.LoadXml(xmlComp);
                            break;

                        case "State":
                            statecomp.LoadStateXml(xmlComp);
                            break;

                        case "Input":
                            entity.AddComponent(InputComponent.Get());
                            break;

                        case "Collision":
                            CollisionComponent coll = new CollisionComponent();
                            coll.LoadXml(xmlComp);
                            entity.AddComponent(coll);
                            break;

                        case "Ladder":
                            LadderComponent ladder = new LadderComponent();
                            ladder.LoadXml(xmlComp);
                            entity.AddComponent(ladder);
                            break;

                        case "Health":
                            HealthComponent health = new HealthComponent();
                            health.LoadXml(xmlComp);
                            entity.AddComponent(health);
                            break;

                        case "Weapons":
                            WeaponComponent weapons = new WeaponComponent();
                            weapons.LoadXml(xmlComp);
                            entity.AddComponent(weapons);
                            break;

                        case "Sounds":
                            SoundComponent sounds = new SoundComponent();
                            sounds.LoadXml(xmlComp);
                            entity.AddComponent(sounds);
                            break;

                        case "Death":
                            foreach (XElement xmlchild in xmlComp.Elements())
                            {
                                switch (xmlchild.Name.LocalName)
                                {
                                    case "Sound":
                                        string soundname = xmlchild.Attribute("name").Value;
                                        bool playing = true;
                                        XAttribute playAttr = xmlchild.Attribute("playing");
                                        if (playAttr != null)
                                        {
                                            if (!bool.TryParse(playAttr.Value, out playing)) throw new EntityXmlException(playAttr, "Playing attribute must be a boolean (true or false).");
                                        }
                                        entity.OnDeath += (e) =>
                                        {
                                            SoundMessage msg = new SoundMessage(e, soundname, playing);
                                            e.SendMessage(msg);
                                        };
                                        break;

                                    case "Spawn":
                                        string ename = xmlchild.Attribute("name").Value;
                                        string statename = "Start";
                                        if (xmlchild.Attribute("state") != null) statename = xmlchild.Attribute("state").Value;
                                        entity.OnDeath += (e) =>
                                        {
                                            GameEntity spawn = e.Spawn(ename);
                                            if (spawn == null) return;
                                            StateMessage msg = new StateMessage(entity, statename);
                                            spawn.SendMessage(msg);
                                        };
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
                                }
                            }
                            break;

                        case "GravityFlip":
                            bool grav;
                            if (!bool.TryParse(xmlComp.Value, out grav)) throw new EntityXmlException(null, (xmlComp as System.Xml.IXmlLineInfo).LineNumber, entity.Name, "GravityFlip", null, "GravityFlip value must represent True or False");
                            entity.GravityFlip = grav;
                            break;
                    }
                }
            }
            catch (EntityXmlException ex)
            {
                ex.Entity = name;
                throw;
            }

            //foreach (System.Drawing.Image img in tilesheets.Values) img.Dispose();

            entities.Add(name, entity);
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
            entity.Group = source.Group;
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
