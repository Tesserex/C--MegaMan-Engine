using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common;
using MegaMan.Common.Entities;

namespace MegaMan.Editor.Controls.ViewModels
{
    class EntityPlacementControlViewModel
    {
        private EntityInfo _entityInfo;

        public EntityPlacement Placement { get; private set; }

        public EntityPlacementControlViewModel(EntityPlacement placement, EntityInfo entityInfo)
        {
            this.Placement = placement;
            this._entityInfo = entityInfo;
        }

        public Sprite DefaultSprite { get { return _entityInfo.DefaultSprite; } }
    }
}
