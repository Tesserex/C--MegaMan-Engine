using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Engine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.StateMachine
{
    public class GameStateMachine
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
                    EndHandler(handler);
                    break;

                case HandlerMode.Push:
                    if (handler.Fade)
                    {
                        if (_handlerStack.Any() && handler.Pause)
                        {
                            var top = _handlerStack.Peek();
                            top.PauseHandler();
                        }

                        Engine.Instance.FadeTransition(() =>
                        {
                            if (_handlerStack.Any() && handler.Pause)
                            {
                                var top = _handlerStack.Peek();
                                top.StopDrawing();
                            }
                            StartHandler(handler);
                            _handlerStack.Peek().PauseHandler();
                        },
                        () =>
                        {
                            _handlerStack.Peek().ResumeHandler();
                        });
                    }
                    else
                    {
                        if (_handlerStack.Any() && handler.Pause)
                        {
                            var top = _handlerStack.Peek();
                            top.PauseHandler();
                            top.StopDrawing();
                        }
                        StartHandler(handler);
                    }
                    break;

                case HandlerMode.Pop:
                    if (handler.Fade)
                    {
                        IGameplayContainer top = null;
                        if (_handlerStack.Any())
                        {
                            top = _handlerStack.Pop();
                            top.PauseHandler();
                            top.End -= ProcessHandler;
                        }
                        Engine.Instance.FadeTransition(() =>
                        {
                            if (top != null) top.StopHandler();
                            if (_handlerStack.Any())
                            {
                                _handlerStack.Peek().StartDrawing();
                            }
                        },
                        () =>
                        {
                            if (_handlerStack.Any())
                            {
                                _handlerStack.Peek().ResumeHandler();
                            }
                        });
                    }
                    else
                    {
                        if (_handlerStack.Any())
                        {
                            var top = _handlerStack.Pop();
                            top.StopHandler();
                            top.End -= ProcessHandler;
                            if (_handlerStack.Any())
                            {
                                _handlerStack.Peek().ResumeHandler();
                            }
                        }
                    }
                    break;
            }
        }

        private void EndHandler(HandlerTransfer nextHandler)
        {
            foreach (var handler in _handlerStack)
            {
                handler.End -= ProcessHandler;
            }

            if (nextHandler.Fade)
                Engine.Instance.FadeTransition(() => EmptyStackAndStart(nextHandler));
            else
                EmptyStackAndStart(nextHandler);
        }

        private void EmptyStackAndStart(HandlerTransfer nextHandler)
        {
            while (_handlerStack.Any())
            {
                _handlerStack.Pop().StopHandler();
            }
            StartHandler(nextHandler);
        }

        private void StartHandler(HandlerTransfer handler)
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

        public void StartStage(string name, string screen = null, Point? startPosition = null)
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

            handler.StartHandler(entityPool);

            _handlerStack.Push(handler);
        }

        public void Unload()
        {
            while (_handlerStack.Any())
            {
                _handlerStack.Pop().StopHandler();
            }
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
#endif
        #endregion
    }
}
