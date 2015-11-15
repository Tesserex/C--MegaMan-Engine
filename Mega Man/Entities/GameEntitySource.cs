using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common.Entities;

namespace MegaMan.Engine.Entities
{
    class GameEntitySource : IEntitySource
    {
        private readonly Dictionary<string, GameEntity> entities = new Dictionary<string, GameEntity>();

        public GameEntity GetOriginalEntity(string name)
        {
            if (!entities.ContainsKey(name)) throw new GameRunException("Someone requested an entity named \"" + name + "\", but I couldn't find it!\n" +
                "You need to make sure it's defined in one of the included XML files.");

            return entities[name];
        }

        internal void LoadEntities(IEnumerable<EntityInfo> entities)
        {
            foreach (var info in entities)
                LoadEntity(info);
        }

        private void LoadEntity(EntityInfo info)
        {
            if (entities.ContainsKey(info.Name))
                throw new GameEntityException("You have defined two entities both named \"" + info.Name + "\".");

            var entity = new GameEntity();
            entity.Name = info.Name;

            entities[info.Name] = entity;

            if (info.SpriteComponent != null)
                LoadSpriteComponent(entity, info.SpriteComponent);
        }

        private void LoadSpriteComponent(GameEntity entity, SpriteComponentInfo componentInfo)
        {
            var spritecomp = new SpriteComponent();
            entity.AddComponent(spritecomp);

            spritecomp.LoadInfo(componentInfo);
        }

        public void LoadEntities(XElement doc)
        {
            foreach (XElement entity in doc.Elements("Entity"))
            {
                LoadEntity(entity);
            }
        }

        private void LoadEntity(XElement xml)
        {
            string name = xml.RequireAttribute("name").Value;

            if (!entities.ContainsKey(name))
                throw new GameRunException("Could not find entity named \"" + name + "\".");

            var entity = entities[name];
            
            entity.MaxAlive = xml.TryAttribute<int>("limit", 50);
            
            PositionComponent poscomp = null;
            StateComponent statecomp = new StateComponent();
            entity.AddComponent(statecomp);

            try
            {
                foreach (XElement xmlComp in xml.Elements())
                {
                    switch (xmlComp.Name.LocalName)
                    {
                        case "EditorData":
                            break;

                        case "Tilesheet":
                            break;

                        case "Trigger":
                            statecomp.LoadStateTrigger(xmlComp);
                            break;

                        case "Sprite":
                            if (poscomp == null)
                            {
                                poscomp = new PositionComponent();
                                entity.AddComponent(poscomp);
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

                        case "Death":
                            entity.OnDeath += EffectParser.LoadTriggerEffect(xmlComp);
                            break;

                        case "GravityFlip":
                            entity.IsGravitySensitive = xmlComp.GetValue<bool>();
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
        }

        public void Unload()
        {
            entities.Clear();
        }
    }
}
