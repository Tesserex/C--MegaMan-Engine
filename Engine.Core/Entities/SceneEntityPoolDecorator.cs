using System.Collections.Generic;

namespace MegaMan.Engine.Entities
{
    public class SceneEntityPoolDecorator : IEntityPool
    {
        private IEntityPool _basePool;

        private readonly List<GameEntity> _additionalEntities;

        public SceneEntityPoolDecorator(IEntityPool basePool)
        {
            _basePool = basePool;
            _additionalEntities = new List<GameEntity>();
        }

        public GameEntity CreateEntity(string name)
        {
            var entity = _basePool.CreateEntity(name);
            _additionalEntities.Add(entity);
            return entity;
        }

        public GameEntity CreateEntityWithId(string id, string name)
        {
            var entity = _basePool.CreateEntityWithId(id, name);
            _additionalEntities.Add(entity);
            return entity;
        }

        public GameEntity GetEntityById(string id)
        {
            return _basePool.GetEntityById(id);
        }

        public int GetNumberAlive(string name)
        {
            return _basePool.GetNumberAlive(name);
        }

        public int GetTotalAlive()
        {
            return _basePool.GetTotalAlive();
        }

        public IEnumerable<IEntity> GetAll()
        {
            return _basePool.GetAll();
        }

        public void RemoveAll()
        {
            foreach (var entity in _additionalEntities)
                entity.Remove();

            _additionalEntities.Clear();
        }
    }
}
