using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MegaMan.Engine
{
    public class Menu : IScreenInformation, IGameplayContainer
    {
        private Dictionary<string, ISceneObject> objects;
        private List<GameEntity> entities;
        private MenuInfo info;
        private MenuStateInfo state;
        private List<MenuOptionCommandInfo> options;
        private int selectedId;
        private Point currentPos;

        public event Action GameThink;
        public event Action GameAct;
        public event Action GameReact;
        public event Action GameCleanup;

        public event Action<HandlerTransfer> End;

        private Menu(MenuInfo info)
        {
            objects = new Dictionary<string, ISceneObject>();
            entities = new List<GameEntity>();
            this.info = info;
        }

        private void ResetState()
        {
            this.options = this.state.Commands.OfType<MenuOptionCommandInfo>().ToList();

            var option = this.options[0];
            this.selectedId = 0;
            this.currentPos = new Point(option.X, option.Y);
        }

        public void StartHandler()
        {
            ResumeHandler();

            this.state = this.info.States[0];
            ResetState();
            RunCommands(this.state.Commands);
        }

        public void PauseHandler()
        {
            Engine.Instance.GameLogicTick -= Tick;
            Engine.Instance.GameRender -= GameRender;
            Engine.Instance.GameInputReceived -= GameInputReceived;
        }

        public void ResumeHandler()
        {
            Engine.Instance.GameLogicTick += Tick;
            Engine.Instance.GameRender += GameRender;
            Engine.Instance.GameInputReceived += GameInputReceived;
        }

        public void StopHandler()
        {
            PauseHandler();

            foreach (var entity in entities)
            {
                entity.Stop();
            }
            entities.Clear();

            foreach (var obj in objects.Values)
            {
                obj.Stop();
            }
            objects.Clear();
        }

        public GameEntity Player
        {
            get { return null; }
        }

        public int TileSize
        {
            get
            {
                return 32;
            }
        }

        public float OffsetX
        {
            get { return 0; }
        }

        public float OffsetY
        {
            get { return 0; }
        }

        public MapSquare SquareAt(int x, int y)
        {
            return null;
        }

        public Tile TileAt(int tx, int ty)
        {
            return null;
        }

        public IEnumerable<MapSquare> Tiles
        {
            get { return null; }
        }

        public void AddSpawnedEntity(GameEntity entity)
        {
            entities.Add(entity);
        }

        public bool IsOnScreen(float x, float y)
        {
            return true;
        }

        public void GameInputReceived(GameInputEventArgs e)
        {
            if (!e.Pressed) return;

            int id = selectedId;
            int min = int.MaxValue;

            if (e.Input == GameInput.Start)
            {
                var select = this.options[selectedId].SelectEvent;
                if (select != null)
                {
                    RunCommands(select);
                }
            }
            else if (e.Input == GameInput.Down)
            {
                for (var i = 0; i < options.Count; i++)
                {
                    if (i == selectedId) continue;

                    var info = options[i];

                    int ydist = info.Y - currentPos.Y;
                    if (ydist == 0) continue;

                    if (ydist < 0) ydist += Game.CurrentGame.PixelsDown;    // wrapping around bottom

                    // weight x distance worse than y distance
                    int dist = 2 * Math.Abs(info.X - currentPos.X) + ydist;
                    if (dist < min)
                    {
                        min = dist;
                        id = i;
                    }
                }
            }
            else if (e.Input == GameInput.Up)
            {
                for (var i = 0; i < options.Count; i++)
                {
                    if (i == selectedId) continue;

                    var info = options[i];
                    int ydist = currentPos.Y - info.Y;
                    if (ydist == 0) continue;

                    if (ydist < 0) ydist += Game.CurrentGame.PixelsDown;    // wrapping around bottom

                    // weight x distance worse than y distance
                    int dist = 2 * Math.Abs(info.X - currentPos.X) + ydist;
                    if (dist < min)
                    {
                        min = dist;
                        id = i;
                    }
                }
            }
            else if (e.Input == GameInput.Right)
            {
                for (var i = 0; i < options.Count; i++)
                {
                    if (i == selectedId) continue;

                    var info = options[i];
                    int xdist = info.X - currentPos.X;
                    if (xdist == 0) continue;

                    if (xdist < 0) xdist += Game.CurrentGame.PixelsAcross;    // wrapping around bottom

                    int dist = 2 * Math.Abs(info.Y - currentPos.Y) + xdist;
                    if (dist < min)
                    {
                        min = dist;
                        id = i;
                    }
                }
            }
            else if (e.Input == GameInput.Left)
            {
                for (var i = 0; i < options.Count; i++)
                {
                    if (i == selectedId) continue;

                    var info = options[i];
                    int xdist = currentPos.X - info.X;
                    if (xdist == 0) continue;

                    if (xdist < 0) xdist += Game.CurrentGame.PixelsAcross;    // wrapping around bottom

                    int dist = 2 * Math.Abs(info.Y - currentPos.Y) + xdist;
                    if (dist < min)
                    {
                        min = dist;
                        id = i;
                    }
                }
            }

            if (id != selectedId)
            {
                SelectOption(id);
            }
        }

        private void SelectOption(int id)
        {
            var off = this.options[selectedId].OffEvent;
            var on = this.options[id].OnEvent;

            if (off != null) RunCommands(off);
            if (on != null) RunCommands(on);

            selectedId = id;
            currentPos = new Point(this.options[id].X, this.options[id].Y);
        }

        public void GameRender(GameRenderEventArgs e)
        {
            foreach (var obj in objects.Values)
            {
                obj.Draw(e.Layers, e.OpacityColor);
            }
        }

        private void Tick(GameTickEventArgs e)
        {
            if (!Game.CurrentGame.Paused)
            {
                if (GameThink != null) GameThink();
                if (GameAct != null) GameAct();
                if (GameReact != null) GameReact();
            }
            if (GameCleanup != null) GameCleanup();
        }

        private void RunCommands(IEnumerable<SceneCommandInfo> commands)
        {
            foreach (var cmd in commands)
            {
                switch (cmd.Type)
                {
                    case SceneCommands.PlayMusic:
                        PlayMusicCommand((ScenePlayCommandInfo)cmd);
                        break;

                    case SceneCommands.Sprite:
                        SpriteCommand((SceneSpriteCommandInfo)cmd);
                        break;

                    case SceneCommands.SpriteMove:
                        SpriteMoveCommand((SceneSpriteMoveCommandInfo)cmd);
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
                }
            }
        }

        private void PlayMusicCommand(ScenePlayCommandInfo command)
        {
            Engine.Instance.SoundSystem.PlayMusicNSF((uint)command.Track);
        }

        private void SpriteCommand(SceneSpriteCommandInfo command)
        {
            var obj = new SceneSprite(info.Sprites[command.Sprite], new Point(command.X, command.Y));
            obj.Start();
            var name = command.Name ?? Guid.NewGuid().ToString();
            if (!objects.ContainsKey(name)) objects.Add(name, obj);
        }

        private void SpriteMoveCommand(SceneSpriteMoveCommandInfo command)
        {
            SceneSprite obj = objects[command.Name] as SceneSprite;
            if (obj != null)
            {
                obj.Move(command.X, command.Y, command.Duration);
                obj.Reset();
            }
        }

        private void TextCommand(SceneTextCommandInfo command)
        {
            var obj = new SceneText(command.Content, command.Speed, command.X, command.Y);
            obj.Start();
            var name = command.Name ?? Guid.NewGuid().ToString();
            if (!objects.ContainsKey(name)) objects.Add(name, obj);
        }

        private void RemoveCommand(SceneRemoveCommandInfo command)
        {
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
            entities.Add(entity);
            entity.Start(this);
        }

        private void FillCommand(SceneFillCommandInfo command)
        {
            Color color = new Color(command.Red, command.Green, command.Blue);

            var obj = new SceneFill(color, command.X, command.Y, command.Width, command.Height, command.Layer);
            obj.Start();
            var name = command.Name ?? Guid.NewGuid().ToString();
            objects.Add(name, obj);
        }

        private void FillMoveCommand(SceneFillMoveCommandInfo command)
        {
            SceneFill obj = objects[command.Name] as SceneFill;
            if (obj != null) obj.Move(command.X, command.Y, command.Width, command.Height, command.Duration);
        }

        private void SoundCommand(SceneSoundCommandInfo command)
        {
            Engine.Instance.SoundSystem.PlaySfx(command.SoundInfo.Name);
        }

        private void NextCommand(SceneNextCommandInfo command)
        {
            if (End != null && command.NextHandler != null)
            {
                End(command.NextHandler);
            }
        }

        private static Dictionary<string, Menu> menus = new Dictionary<string, Menu>();

        public static void Load(XElement node)
        {
            var info = MenuInfo.FromXml(node, Game.CurrentGame.BasePath);

            if (menus.ContainsKey(info.Name)) throw new GameXmlException(node, String.Format("You have two Menus with the name of {0} - names must be unique.", info.Name));

            menus.Add(info.Name, new Menu(info));
        }

        public static Menu Get(string name)
        {
            if (!menus.ContainsKey(name))
            {
                throw new GameRunException(
                    String.Format("I tried to run the scene named '{0}', but couldn't find it.\nPerhaps it's not being included in the main file.", name)
                );
            }

            return menus[name];
        }

        public static void Unload()
        {
            menus.Clear();
        }
    }
}
