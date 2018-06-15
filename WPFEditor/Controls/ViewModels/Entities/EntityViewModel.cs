using MegaMan.Common.Entities;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls.ViewModels.Entities
{
    public class EntityViewModel
    {
        private readonly EntityInfo _entity;
        private readonly ProjectDocument _project;

        public EntityInfo Entity { get { return _entity; } }
            
        public EntityViewModel(EntityInfo entity, ProjectDocument project)
        {
            _entity = entity;
            _project = project;
            DefaultSprite = SpriteModel.ForEntity(_entity, _project);
        }

        public string Name { get { return _entity.Name; } }

        public IEntityImage DefaultSprite { get; private set; }
    }
}
