using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine.StateMachine
{
    public class GameStateMachine : IStateMachine
    {
        private readonly Stack<IGameplayContainer> _handlerStack;
        private readonly IEntityPool _entityPool;
        private readonly StageFactory _stageFactory;

        public GameStateMachine(IEntityPool entityPool, StageFactory stageFactory)
        {
            _handlerStack = new Stack<IGameplayContainer>();
            _entityPool = entityPool;
            _stageFactory = stageFactory;
        }

        public void ProcessHandler(HandlerTransfer handler)
        {
            switch (handler.Mode)
            {
                case HandlerMode.Next:
                    new EndStateTransition(handler).Apply(this);
                    break;

                case HandlerMode.Push:
                    new PushStateTransition(handler).Apply(this);
                    break;

                case HandlerMode.Pop:
                    new PopStateTransition(handler).Apply(this);
                    break;
            }
        }

        public void PauseTopOfStack()
        {
            if (_handlerStack.Any())
            {
                _handlerStack.Peek().PauseHandler();
            }
        }

        public void ResumeTopOfStack()
        {
            if (_handlerStack.Any())
                _handlerStack.Peek().ResumeHandler();
        }

        public void StopAllHandlers()
        {
            while (_handlerStack.Any())
            {
                _handlerStack.Pop().StopHandler();
            }
        }

        public void StopAllInput()
        {
            foreach (var handler in _handlerStack)
            {
                handler.StopInput();
            }
        }

        public void StartHandler(HandlerTransfer handler)
        {
            if (handler != null)
            {
                switch (handler.Type)
                {
                    case HandlerType.Scene:
                        StartScene(handler);
                        break;

                    case HandlerType.Stage:
                        StartStage(handler.Name);
                        break;

                    case HandlerType.Menu:
                        StartMenu(handler);
                        break;
                }
            }
        }

        public void StartMenu(HandlerTransfer handler)
        {
            var menu = Menu.Get(handler.Name);
            StartAndPushToStack(menu);
        }

        public void StartScene(HandlerTransfer handler)
        {
            var scene = Scene.Get(handler.Name);
            StartAndPushToStack(scene);
        }

        public void StartStage(string name, string? screen = null, Point? startPosition = null)
        {
            var stage = _stageFactory.Get(name);

            if (screen != null && startPosition != null)
                stage.SetTestingStartPosition(screen, startPosition.Value);

            StartAndPushToStack(stage);
        }

        private void StartAndPushToStack(GameHandler handler)
        {
            handler.End += ProcessHandler;

            var entityPool = _entityPool;

            if (_handlerStack.Any())
                entityPool = new SceneEntityPoolDecorator(_handlerStack.Peek().Entities);

            _handlerStack.Push(handler);

            handler.StartHandler(entityPool);
        }

        #region Debug
#if DEBUG
        public void DebugEmptyHealth()
        {
            if (_handlerStack.Count == 0) return;

            var map = _handlerStack.Peek() as StageHandler;
            if (map != null)
            {
                map.Player.SendMessage(new DamageMessage(null, float.PositiveInfinity));
            }
        }

        public void DebugFillHealth()
        {
            if (_handlerStack.Count == 0) return;

            var map = _handlerStack.Peek() as StageHandler;
            if (map != null)
            {
                map.Player.SendMessage(new HealMessage(null, float.PositiveInfinity));
            }
        }

        public void DebugEmptyWeapon()
        {
            if (_handlerStack.Count == 0) return;

            var map = _handlerStack.Peek() as StageHandler;
            if (map != null)
            {
                var weaponComponent = map.Player.GetComponent<WeaponComponent>();
                if (weaponComponent != null)
                {
                    weaponComponent.AddAmmo(-1 * weaponComponent.Ammo(weaponComponent.CurrentWeapon));
                }
            }
        }

        public void DebugFillWeapon()
        {
            if (_handlerStack.Count == 0) return;

            var map = _handlerStack.Peek() as StageHandler;
            if (map != null)
            {
                var weaponComponent = map.Player.GetComponent<WeaponComponent>();
                if (weaponComponent != null)
                {
                    weaponComponent.AddAmmo(weaponComponent.MaxAmmo(weaponComponent.CurrentWeapon));
                }
            }
        }

        public bool DebugFlipGravity()
        {
            if (_handlerStack.Any())
            {
                var top = _handlerStack.Peek();
                top.IsGravityFlipped = !top.IsGravityFlipped;
                return top.IsGravityFlipped;
            }
            return false;
        }

        public bool GetFlipGravity()
        {
            if (!_handlerStack.Any()) return false;

            var top = _handlerStack.Peek();
            return top.IsGravityFlipped;
        }
#endif
        #endregion

        public void RemoveAllEndHandlers()
        {
            foreach (var handler in _handlerStack)
            {
                handler.End -= ProcessHandler;
            }
        }

        public void Push(IGameplayContainer handler)
        {
            _handlerStack.Push(handler);
        }

        public void PauseDrawingTopOfStack()
        {
            if (_handlerStack.Any())
            {
                _handlerStack.Peek().StopDrawing();
            }
        }

        public void ResumeDrawingTopOfStack()
        {
            if (_handlerStack.Any())
            {
                _handlerStack.Peek().StartDrawing();
            }
        }

        public void FinalizeTopHandler()
        {
            if (_handlerStack.Any())
            {
                var top = _handlerStack.Peek();
                top.PauseHandler();
                top.End -= ProcessHandler;
            }
        }

        public void RemoveTopHandler()
        {
            if (_handlerStack.Any())
            {
                var top = _handlerStack.Pop();
                top.StopHandler();
                top.End -= ProcessHandler;
            }
        }
    }
}
