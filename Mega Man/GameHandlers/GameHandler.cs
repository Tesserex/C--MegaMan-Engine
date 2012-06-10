using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;
using Microsoft.Xna.Framework;

namespace MegaMan.Engine
{
    public abstract class GameHandler : IGameplayContainer
    {
        protected HandlerInfo Info { get; set; }

        protected Dictionary<string, IHandlerObject> objects;

        public IEntityContainer Entities { get; set; }

        public event Action GameThink;
        public event Action GameAct;
        public event Action GameReact;
        public event Action GameCleanup;
        public event Action<HandlerTransfer> End;
        public event GameRenderEventHandler Draw;

        private bool running;

        public virtual void StartHandler()
        {
            if (Entities == null)
            {
                Entities = new SceneEntities();
            }

            ResumeHandler();
            StartDrawing();
        }

        public void PauseHandler()
        {
            if (!running) return;
            Engine.Instance.GameLogicTick -= Tick;
            Engine.Instance.GameInputReceived -= GameInputReceived;
            running = false;
        }

        public void ResumeHandler()
        {
            if (running) return;
            Engine.Instance.GameLogicTick += Tick;
            Engine.Instance.GameInputReceived += GameInputReceived;
            running = true;
        }

        public void StopHandler()
        {
            PauseHandler();
            StopDrawing();

            Entities.ClearEntities();

            foreach (var obj in objects.Values)
            {
                obj.Stop();
            }
            objects.Clear();
        }

        public virtual void StopDrawing()
        {
            Engine.Instance.GameRender -= GameRender;
        }

        public virtual void StartDrawing()
        {
            Engine.Instance.GameRender += GameRender;
        }

        protected abstract void GameInputReceived(GameInputEventArgs e);

        protected virtual void Tick(GameTickEventArgs e)
        {
            if (GameThink != null) GameThink();
            if (GameAct != null) GameAct();
            if (GameReact != null) GameReact();
            if (GameCleanup != null) GameCleanup();
        }

        protected virtual void GameRender(GameRenderEventArgs e)
        {
            foreach (var obj in objects.Values)
            {
                obj.Draw(e.Layers, e.OpacityColor);
            }

            if (Draw != null) Draw(e);
        }

        protected void Finish(HandlerTransfer transfer)
        {
            if (End != null)
            {
                End(transfer);
            }
        }

        protected void RunCommands(IEnumerable<SceneCommandInfo> commands)
        {
            foreach (var cmd in commands)
            {
                switch (cmd.Type)
                {
                    case SceneCommands.PlayMusic:
                        PlayMusicCommand((ScenePlayCommandInfo)cmd);
                        break;

                    case SceneCommands.Add:
                        AddCommand((SceneAddCommandInfo)cmd);
                        break;

                    case SceneCommands.Move:
                        MoveCommand((SceneMoveCommandInfo)cmd);
                        break;

                    case SceneCommands.Remove:
                        RemoveCommand((SceneRemoveCommandInfo)cmd);
                        break;

                    case SceneCommands.Entity:
                        EntityCommand((SceneEntityCommandInfo)cmd);
                        break;

                    case SceneCommands.Text:
                        TextCommand((SceneTextCommandInfo)cmd);
                        break;

                    case SceneCommands.Fill:
                        FillCommand((SceneFillCommandInfo)cmd);
                        break;

                    case SceneCommands.FillMove:
                        FillMoveCommand((SceneFillMoveCommandInfo)cmd);
                        break;

                    case SceneCommands.Sound:
                        SoundCommand((SceneSoundCommandInfo)cmd);
                        break;

                    case SceneCommands.Next:
                        NextCommand((SceneNextCommandInfo)cmd);
                        break;

                    case SceneCommands.Call:
                        CallCommand((SceneCallCommandInfo)cmd);
                        break;

                    case SceneCommands.Effect:
                        EffectCommand((SceneEffectCommandInfo)cmd);
                        break;
                }
            }
        }

        private void PlayMusicCommand(ScenePlayCommandInfo command)
        {
            Engine.Instance.SoundSystem.PlayMusicNSF((uint)command.Track);
        }

        private void AddCommand(SceneAddCommandInfo command)
        {
            var obj = Info.Objects[command.Object];

            IHandlerObject handler = null;
            if (obj is HandlerSpriteInfo)
            {
                handler = new HandlerSprite(((HandlerSpriteInfo)obj).Sprite, new Point(command.X, command.Y));
            }
            else if (obj is MeterInfo)
            {
                handler = new HandlerMeter(HealthMeter.Create((MeterInfo)obj, false), this);
            }
            handler.Start();
            var name = command.Name ?? Guid.NewGuid().ToString();
            if (!objects.ContainsKey(name)) objects.Add(name, handler);
        }

        private void TextCommand(SceneTextCommandInfo command)
        {
            var obj = new HandlerText(command, Entities);
            obj.Start();
            var name = command.Name ?? Guid.NewGuid().ToString();
            if (!objects.ContainsKey(name)) objects.Add(name, obj);
        }

        private void RemoveCommand(SceneRemoveCommandInfo command)
        {
            if (!objects.ContainsKey(command.Name))
            {
                throw new GameRunException(String.Format("The handler '{0}' referenced an object called '{1}', which doesn't exist.", Info.Name, command.Name));
            }

            objects[command.Name].Stop();
            objects.Remove(command.Name);
        }

        private void EntityCommand(SceneEntityCommandInfo command)
        {
            var entity = GameEntity.Get(command.Entity, this);
            entity.GetComponent<PositionComponent>().SetPosition(command.X, command.Y);
            if (!string.IsNullOrEmpty(command.State))
            {
                entity.SendMessage(new StateMessage(null, command.State));
            }
            Entities.AddEntity(entity);
            entity.Start();
        }

        private void FillCommand(SceneFillCommandInfo command)
        {
            Color color = new Color(command.Red, command.Green, command.Blue);

            var obj = new HandlerFill(color, command.X, command.Y, command.Width, command.Height, command.Layer);
            obj.Start();
            var name = command.Name ?? Guid.NewGuid().ToString();
            if (!objects.ContainsKey(name)) objects.Add(name, obj);
        }

        private void CallCommand(SceneCallCommandInfo command)
        {
            var player = Entities.GetEntities("Player").FirstOrDefault();

            if (player != null)
            {
                EffectParser.GetLateBoundEffect(command.Name)(player);
            }
        }

        private void MoveCommand(SceneMoveCommandInfo command)
        {
            HandlerSprite obj = objects[command.Name] as HandlerSprite;
            if (obj != null)
            {
                obj.Move(command.X, command.Y, command.Duration);
                obj.Reset();
            }
        }

        private void FillMoveCommand(SceneFillMoveCommandInfo command)
        {
            HandlerFill obj = objects[command.Name] as HandlerFill;
            if (obj != null) obj.Move(command.X, command.Y, command.Width, command.Height, command.Duration);
        }

        private void SoundCommand(SceneSoundCommandInfo command)
        {
            Engine.Instance.SoundSystem.PlaySfx(command.SoundInfo.Name);
        }

        private void NextCommand(SceneNextCommandInfo command)
        {
            if (command.NextHandler != null)
            {
                Finish(command.NextHandler);
            }
        }

        private void EffectCommand(SceneEffectCommandInfo command)
        {
            var entity = Entities.GetEntities(command.EntityName).FirstOrDefault();

            if (entity != null)
            {
                var effect = EffectParser.GetOrLoadEffect(command.GeneratedName, command.EffectNode);
                effect(entity);
            }
        }
    }
}
