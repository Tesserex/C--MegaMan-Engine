﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaMan.Engine.Entities
{
    class GameEntityPool : IEntityPool
    {
        private readonly IEntitySource _entitySource;
        private readonly Dictionary<string, GameEntity> entitiesInUse = new Dictionary<string, GameEntity>();
        private readonly Dictionary<string, Stack<GameEntity>> deadEntityPool = new Dictionary<string, Stack<GameEntity>>();

        private bool _enumeratingActives;

        public GameEntityPool(IEntitySource entitySource)
        {
            _entitySource = entitySource;
        }

        public GameEntity CreateEntity(string name)
        {
            return CreateEntityWithId(Guid.NewGuid().ToString(), name);
        }

        public GameEntity CreateEntityWithId(string id, string name)
        {
            // look in the pool
            if (deadEntityPool.ContainsKey(name) && deadEntityPool[name].Any())
            {
                var entity = deadEntityPool[name].Pop();
                BindEntityEventRegistration(id, entity);
                return entity;
            }
            else
            {
                // clone it
                var entity = new GameEntity();
                var source = _entitySource.GetOriginalEntity(name);

                if (GetNumberAlive(name) >= source.MaxAlive) return null;

                foreach (var c in source.Components)
                {
                    entity.AddComponent(c.Clone());
                }
                entity.Name = source.Name;
                entity.OnDeath = source.OnDeath;
                entity.IsGravitySensitive = source.IsGravitySensitive;

                BindEntityEventRegistration(id, entity);

                return entity;
            }
        }

        private void BindEntityEventRegistration(string id, GameEntity entity)
        {
            entitiesInUse.Add(id, entity);

            Action removalAction = () => { };
            removalAction = () =>
            {
                RemoveEntity(id, entity);
                entity.Removed -= removalAction;
            };
            entity.Removed += removalAction;
        }

        public int GetNumberAlive(string name)
        {
            return entitiesInUse.Values.Count(e => e.Name == name && e.Running);
        }

        public int GetTotalAlive()
        {
            return entitiesInUse.Values.Count(e => e.Running);
        }

        private void RemoveEntity(string id, GameEntity entity)
        {
            if (_enumeratingActives == false)
                entitiesInUse.Remove(id);

            if (!deadEntityPool.ContainsKey(entity.Name))
            {
                deadEntityPool[entity.Name] = new Stack<GameEntity>();
            }

            deadEntityPool[entity.Name].Push(entity);
        }

        public IEnumerable<IEntity> GetAll()
        {
            return entitiesInUse.Values;
        }

        public void RemoveAll()
        {
            _enumeratingActives = true;

            foreach (var entity in entitiesInUse.Values)
                entity.Remove();

            entitiesInUse.Clear();

            _enumeratingActives = false;
        }

        public void UnloadAll()
        {
            RemoveAll();

            deadEntityPool.Clear();

            EffectParser.Unload();
        }


        public GameEntity GetEntityById(string id)
        {
            if (id != null && entitiesInUse.ContainsKey(id))
                return entitiesInUse[id];
            return null;
        }
    }
}
