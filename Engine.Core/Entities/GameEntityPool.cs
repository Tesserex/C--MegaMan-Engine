using System.Collections.Concurrent;

namespace MegaMan.Engine.Entities
{
    class GameEntityPool : IEntityPool
    {
        private readonly IEntitySource _entitySource;
        private readonly ConcurrentDictionary<string, GameEntity> entitiesInUse = new ConcurrentDictionary<string, GameEntity>();
        private readonly ConcurrentDictionary<string, Stack<GameEntity>> deadEntityPool = new ConcurrentDictionary<string, Stack<GameEntity>>();

        private bool removingAll;

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
            entitiesInUse.AddOrUpdate(id, (k) => entity, (k,e) => entity);

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
            if (!removingAll)
                entitiesInUse.Remove(id, out _);

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
            removingAll = true;

            foreach (var entity in entitiesInUse.Values)
                entity.Remove();

            entitiesInUse.Clear();

            removingAll = false;
        }

        public void UnloadAll()
        {
            RemoveAll();

            deadEntityPool.Clear();

            EffectParser.Unload();
        }


        public GameEntity? GetEntityById(string id)
        {
            if (id != null && entitiesInUse.ContainsKey(id))
                return entitiesInUse[id];
            return null;
        }
    }
}
