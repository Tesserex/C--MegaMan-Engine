using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common;
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
        }

        public string Name { get { return _entity.Name; } }

        public SpriteModel DefaultSprite
        {
            get { return SpriteModel.ForEntity(_entity, _project); }
        }
    }
}
