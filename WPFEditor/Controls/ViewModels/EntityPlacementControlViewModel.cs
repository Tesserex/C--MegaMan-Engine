using System;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls.ViewModels
{
    class EntityPlacementControlViewModel : ViewModelBase
    {
        private EntityInfo _entityInfo;
        private ScreenDocument _screen;

        private bool _hovered;
        public bool Hovered
        {
            get { return _hovered; }
            set
            {
                _hovered = value;
                OnPropertyChanged("BorderColor");
            }
        }

        public EntityPlacement Placement { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public EntityPlacementControlViewModel(EntityPlacement placement, EntityInfo entityInfo, ScreenDocument screen)
        {
            this.Placement = placement;
            this._entityInfo = entityInfo;
            this._screen = screen;
            DeleteCommand = new RelayCommand(Delete);
            ViewModelMediator.Current.GetEvent<ZoomChangedEventArgs>().Subscribe(ZoomChanged);
        }

        private void Delete(object obj)
        {
            _screen.RemoveEntity(Placement);
            _screen.Stage.PushHistoryAction(new RemoveEntityAction(Placement, _screen));
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
                return Hovered ? "Cyan" : "Transparent";
            }
        }
    }
}
