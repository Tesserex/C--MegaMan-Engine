using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Entities.Effects;
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
        public SpriteModel DefaultSprite { get; private set; }

        public ICommand DeleteCommand { get; private set; }
        public ICommand FlipCommand { get; private set; }
        public ICommand RespawnCommand { get; private set; }
        public ICommand StartStateCommand { get; private set; }

        public event EventHandler PlacementModified;

        public EntityPlacementControlViewModel(EntityPlacement placement, EntityInfo entityInfo, ScreenDocument screen)
        {
            if (placement == null)
                throw new ArgumentNullException("placement");

            if (entityInfo == null)
                throw new ArgumentNullException("entityInfo");

            if (screen == null)
                throw new ArgumentNullException("screen");

            this.Placement = placement;
            this._entityInfo = entityInfo;
            this._screen = screen;
            this.DefaultSprite = this.GetDefaultSprite();
            this.DefaultSprite.Play();

            DeleteCommand = new RelayCommand(Delete);
            FlipCommand = new RelayCommand(Flip);
            RespawnCommand = new RelayCommand(SetRespawnMode);
            StartStateCommand = new RelayCommand(SetStartState);

            ViewModelMediator.Current.GetEvent<ZoomChangedEventArgs>().Subscribe(ZoomChanged);
        }

        public void Destroy()
        {
            ViewModelMediator.Current.GetEvent<ZoomChangedEventArgs>().Unsubscribe(ZoomChanged);
        }

        private void SetStartState(object obj)
        {
            Placement.state = obj.ToString();
            _screen.Stage.Dirty = true;
            OnPropertyChanged("StartState");
            OnPropertyChanged("DefaultSprite");
        }

        private void SetRespawnMode(object obj)
        {
            var mode = (RespawnBehavior)Enum.Parse(typeof(RespawnBehavior), obj.ToString());
            Placement.respawn = mode;
            _screen.Stage.Dirty = true;

            OnPropertyChanged("RespawnsOffscreen");
            OnPropertyChanged("RespawnsDeath");
            OnPropertyChanged("RespawnsStage");
            OnPropertyChanged("RespawnsNever");
        }

        private void Flip(object obj)
        {
            Placement.direction = (Placement.direction != Direction.Left) ? Direction.Left : Direction.Right;
            _screen.Stage.Dirty = true;
            OnPropertyChanged("Flipped");
            if (PlacementModified != null)
                PlacementModified(this, new EventArgs());
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

        private SpriteModel GetDefaultSprite()
        {
            var hasSprites = _entityInfo.SpriteComponent != null && _entityInfo.SpriteComponent.Sprites.Any();

            if (hasSprites)
            {
                var state = _entityInfo.StateComponent.States.SingleOrDefault(s => s.Name == StartState);
                if (state != null)
                {
                    var stateSprite = state.Initializer.Parts.OfType<SpriteEffectPartInfo>().FirstOrDefault();
                    if (stateSprite != null && stateSprite.Name != null && _entityInfo.SpriteComponent.Sprites.ContainsKey(stateSprite.Name))
                        return new SpriteModel(_entityInfo.SpriteComponent.Sprites[stateSprite.Name]);
                }
            }

            return SpriteModel.ForEntity(_entityInfo, _screen.Stage.Project);
        }

        public double Zoom { get { return Convert.ToDouble(App.Current.Resources["Zoom"] ?? 1); } }

        public bool Flipped { get { return (Placement.direction == Direction.Left); } }

        public string BorderColor
        {
            get
            {
                return Hovered ? "Cyan" : "Transparent";
            }
        }

        public IEnumerable<string> States
        {
            get
            {
                return _entityInfo.StateComponent.States.Select(s => s.Name);
            }
        }

        public string StartState { get { return Placement.state ?? States.FirstOrDefault() ?? "Start"; } }

        public bool RespawnsOffscreen { get { return Placement.respawn == RespawnBehavior.Offscreen; } }
        public bool RespawnsDeath { get { return Placement.respawn == RespawnBehavior.Death; } }
        public bool RespawnsStage { get { return Placement.respawn == RespawnBehavior.Stage; } }
        public bool RespawnsNever { get { return Placement.respawn == RespawnBehavior.Never; } }
    }
}
