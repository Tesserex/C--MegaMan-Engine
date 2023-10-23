using System.Collections.Generic;
using MegaMan.Common.Entities;
using MegaMan.IO.Xml;

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

            entity.IsGravitySensitive = info.GravityFlip;

            if (info.Death != null)
                entity.OnDeath = EffectParser.LoadTriggerEffect(info.Death);

            if (info.SpriteComponent != null)
                LoadSpriteComponent(entity, info.SpriteComponent);

            if (info.PositionComponent != null || info.SpriteComponent != null)
                LoadPositionComponent(entity, info.PositionComponent);

            if (info.MovementComponent != null)
                LoadMovementComponent(info.MovementComponent, entity);

            if (info.InputComponent != null)
                entity.AddComponent(new InputComponent());

            if (info.CollisionComponent != null)
                LoadCollisionComponent(entity, info.CollisionComponent);

            if (info.StateComponent != null)
                LoadStateComponent(entity, info.StateComponent);

            if (info.HealthComponent != null)
                LoadHealthComponent(entity, info.HealthComponent);

            if (info.WeaponComponent != null)
                LoadWeaponComponent(entity, info.WeaponComponent);

            if (info.LadderComponent != null)
                LoadLadderComponent(entity, info.LadderComponent);

            // everyone gets these
            entity.AddComponent(new SoundComponent());
            entity.AddComponent(new TimerComponent());
            entity.AddComponent(new VarsComponent());
        }

        private void LoadLadderComponent(GameEntity entity, LadderComponentInfo info)
        {
            var comp = new LadderComponent();
            entity.AddComponent(comp);
            comp.LoadInfo(info);
        }

        private void LoadWeaponComponent(GameEntity entity, WeaponComponentInfo info)
        {
            var comp = new WeaponComponent();
            entity.AddComponent(comp);
            comp.LoadInfo(info);
        }

        private void LoadHealthComponent(GameEntity entity, HealthComponentInfo info)
        {
            var comp = new HealthComponent();
            entity.AddComponent(comp);
            comp.LoadInfo(info);
        }

        private static void LoadMovementComponent(MovementComponentInfo info, GameEntity entity)
        {
            var moveComp = new MovementComponent();
            entity.AddComponent(moveComp);
            moveComp.LoadInfo(info);
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

        public void Unload()
        {
            entities.Clear();
        }
    }
}
