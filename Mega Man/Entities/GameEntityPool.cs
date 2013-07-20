using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Entities
{
    class GameEntityPool : IEntityPool
    {
        private readonly IEntitySource _entitySource;
        private readonly Dictionary<string, Int32> entityCounts = new Dictionary<string, Int32>();
        private readonly List<GameEntity> actives = new List<GameEntity>();
        private readonly Dictionary<string, Stack<GameEntity>> deadEntityPool = new Dictionary<string, Stack<GameEntity>>();
        private readonly List<Tuple<string, string, int>> neverRespawnable = new List<Tuple<string, string, int>>();

        private bool _enumeratingActives = false;

        public GameEntityPool(IEntitySource entitySource)
        {
            _entitySource = entitySource;
        }

        public GameEntity CreateEntity(string name)
        {
            // look in the pool
            if (deadEntityPool.ContainsKey(name))
            {
                if (deadEntityPool[name].Any())
                {
                    return deadEntityPool[name].Pop();
                }
            }

            // clone it
            GameEntity entity = new GameEntity(this);
            GameEntity source = _entitySource.GetOriginalEntity(name);

            if (GetNumberAlive(name) >= source.MaxAlive) return null;

            foreach (Component c in source.Components)
            {
                entity.AddComponent(c.Clone());
            }
            entity.Name = source.Name;
            entity.OnDeath = source.OnDeath;
            entity.GravityFlip = source.GravityFlip;

            BindEntityStartAndStopRegistration(entity);

            return entity;
        }

        private void BindEntityStartAndStopRegistration(GameEntity entity)
        {
            entity.Started += () => RegisterEntity(entity);
            entity.Stopped += () => RemoveEntity(entity);
        }

        public int GetNumberAlive(string name)
        {
            if (!entityCounts.ContainsKey(name))
                entityCounts[name] = 0;

            return entityCounts[name];
        }

        private void AdjustNumberAlive(string name, Int32 change)
        {
            if (!entityCounts.ContainsKey(name))
                entityCounts[name] = 0;

            entityCounts[name] += change;
        }

        public int GetTotalAlive()
        {
            return actives.Count;
        }

        public void NeverRespawnAgain(string stage, string screen, int index)
        {
            neverRespawnable.Add(new Tuple<string, string, int>(stage, screen, index));
        }

        public bool Respawnable(string stage, string screen, int index)
        {
            return !neverRespawnable.Contains(new Tuple<string, string, int>(stage, screen, index));
        }

        private void RegisterEntity(GameEntity entity)
        {
            actives.Add(entity);
            AdjustNumberAlive(entity.Name, 1);
        }

        private void RemoveEntity(GameEntity entity)
        {
            if (_enumeratingActives == false)
                actives.Remove(entity);

            AdjustNumberAlive(entity.Name, -1);

            if (!deadEntityPool.ContainsKey(entity.Name))
            {
                deadEntityPool[entity.Name] = new Stack<GameEntity>();
            }

            deadEntityPool[entity.Name].Push(entity);
        }

        public IEnumerable<GameEntity> GetAll()
        {
            return actives;
        }

        public void StopAll()
        {
            _enumeratingActives = true;

            foreach (GameEntity entity in actives) entity.Stop();
            actives.Clear();

            _enumeratingActives = false;
        }

        public void UnloadAll()
        {
            StopAll();

            deadEntityPool.Clear();

            neverRespawnable.Clear();

            EffectParser.Unload();
        }
    }
}
