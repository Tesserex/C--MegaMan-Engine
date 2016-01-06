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

            if (info.PositionComponent != null || info.SpriteComponent != null)
                LoadPositionComponent(entity, info.PositionComponent);

            if (info.InputComponent != null)
                entity.AddComponent(new InputComponent());

            if (info.CollisionComponent != null)
                LoadCollisionComponent(entity, info.CollisionComponent);

            if (info.StateComponent != null)
                LoadStateComponent(entity, info.StateComponent);
        }

        private void LoadStateComponent(GameEntity entity, StateComponentInfo info)
        {
            var comp = new StateComponent();
            entity.AddComponent(comp);
            comp.LoadInfo(info);
        }

        private void LoadCollisionComponent(GameEntity entity, CollisionComponentInfo info)
        {
            var comp = new CollisionComponent();
            entity.AddComponent(comp);
            comp.Loadinfo(info);
        }

        private void LoadSpriteComponent(GameEntity entity, SpriteComponentInfo componentInfo)
        {
            var spritecomp = new SpriteComponent();
            entity.AddComponent(spritecomp);

            spritecomp.LoadInfo(componentInfo);
        }

        private void LoadPositionComponent(GameEntity entity, PositionComponentInfo componentInfo)
        {
            var poscomp = new PositionComponent();
            entity.AddComponent(poscomp);

            if (componentInfo != null)
                poscomp.LoadInfo(componentInfo);
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

            try
            {
                foreach (XElement xmlComp in xml.Elements())
                {
                    switch (xmlComp.Name.LocalName)
                    {
                        case "EditorData":
                        case "Tilesheet":
                        case "Sprite":
                        case "Position":
                        case "Input":
                        case "Collision":
                        case "State":
                        case "Trigger":
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
