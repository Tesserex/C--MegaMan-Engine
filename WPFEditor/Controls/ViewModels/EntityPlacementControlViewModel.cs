using System;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls.ViewModels
{
    class EntityPlacementControlViewModel : ViewModelBase
    {
        private EntityInfo _entityInfo;

        public EntityPlacement Placement { get; private set; }

        public EntityPlacementControlViewModel(EntityPlacement placement, EntityInfo entityInfo)
        {
            this.Placement = placement;
            this._entityInfo = entityInfo;
            ViewModelMediator.Current.GetEvent<ZoomChangedEventArgs>().Subscribe(ZoomChanged);
        }

        private void ZoomChanged(object sender, ZoomChangedEventArgs e)
        {
            OnPropertyChanged("Zoom");
        }

        public Sprite DefaultSprite { get { return _entityInfo.DefaultSprite; } }

        public double Zoom { get { return Convert.ToDouble(App.Current.Resources["Zoom"] ?? 1); } }

        public string BorderColor
        {
            get
            {
                return "Cyan";
            }
        }
    }
}
