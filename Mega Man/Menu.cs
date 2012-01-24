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
            Engine.Instance.GameLogicTick += Tick;
            Engine.Instance.GameRender += GameRender;
            Engine.Instance.GameInputReceived += GameInputReceived;

            this.state = this.info.States[0];
            ResetState();
            SetupState();
        }

        public void StopHandler()
        {
            Engine.Instance.GameLogicTick -= Tick;
            Engine.Instance.GameRender -= GameRender;
            Engine.Instance.GameInputReceived -= GameInputReceived;
            foreach (var entity in entities)
            {
                entity.Stop();
            }
            entities.Clear();

            foreach (var obj in objects.Values)
            {
                obj.Stop();
            }
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
            Point nextPos = currentPos;
            int min = int.MaxValue;

            if (e.Input == GameInput.Start)
            {
                var next = this.options[selectedId].NextHandler;
                if (next != null && End != null)
                {
                    End(next);
                    return;
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
                        nextPos = new Point(info.X, info.Y);
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
                        nextPos = new Point(info.X, info.Y);
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
                        nextPos = new Point(info.X, info.Y);
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
                        nextPos = new Point(info.X, info.Y);
                    }
                }
            }

            if (id != selectedId)
            {
                selectedId = id;
                currentPos = nextPos;
            }
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

        private void SetupState()
        {
            foreach (var cmd in this.state.Commands)
            {
                switch (cmd.Type)
                {
                    case KeyFrameCommands.PlayMusic:
                        PlayMusicCommand((KeyFramePlayCommandInfo)cmd);
                        break;

                    case KeyFrameCommands.Sprite:
                        SpriteCommand((KeyFrameSpriteCommandInfo)cmd);
                        break;

                    case KeyFrameCommands.Remove:
                        RemoveCommand((KeyFrameRemoveCommandInfo)cmd);
                        break;

                    case KeyFrameCommands.Entity:
                        EntityCommand((KeyFrameEntityCommandInfo)cmd);
                        break;

                    case KeyFrameCommands.Text:
                        TextCommand((KeyFrameTextCommandInfo)cmd);
                        break;

                    case KeyFrameCommands.Fill:
                        FillCommand((KeyFrameFillCommandInfo)cmd);
                        break;

                    case KeyFrameCommands.FillMove:
                        FillMoveCommand((KeyFrameFillMoveCommandInfo)cmd);
                        break;
                }
            }
        }

        private void PlayMusicCommand(KeyFramePlayCommandInfo command)
        {
            Engine.Instance.SoundSystem.PlayMusicNSF((uint)command.Track);
        }

        private void SpriteCommand(KeyFrameSpriteCommandInfo command)
        {
            var obj = new SceneSprite(info.Sprites[command.Sprite], new Point(command.X, command.Y));
            obj.Start();
            var name = command.Name ?? Guid.NewGuid().ToString();
            objects.Add(name, obj);
        }

        private void TextCommand(KeyFrameTextCommandInfo command)
        {
            var obj = new SceneText(command.Content, command.Speed, command.X, command.Y);
            obj.Start();
            var name = command.Name ?? Guid.NewGuid().ToString();
            objects.Add(name, obj);
        }

        private void RemoveCommand(KeyFrameRemoveCommandInfo command)
        {
            objects[command.Name].Stop();
            objects.Remove(command.Name);
        }

        private void EntityCommand(KeyFrameEntityCommandInfo command)
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

        private void FillCommand(KeyFrameFillCommandInfo command)
        {
            Color color = new Color(command.Red, command.Green, command.Blue);

            var obj = new SceneFill(color, command.X, command.Y, command.Width, command.Height, command.Layer);
            obj.Start();
            var name = command.Name ?? Guid.NewGuid().ToString();
            objects.Add(name, obj);
        }

        private void FillMoveCommand(KeyFrameFillMoveCommandInfo command)
        {
            SceneFill obj = objects[command.Name] as SceneFill;
            obj.Move(command.X, command.Y, command.Width, command.Height, command.Duration);
        }

        private static Dictionary<string, Menu> menus = new Dictionary<string, Menu>();

        public static void Load(XElement node)
        {
            var info = MenuInfo.FromXml(node, Game.CurrentGame.BasePath);
            menus.Add(info.Name, new Menu(info));
        }

        public static Menu Get(string name)
        {
            return menus[name];
        }

        public static void Unload()
        {
            menus.Clear();
        }
    }
}
