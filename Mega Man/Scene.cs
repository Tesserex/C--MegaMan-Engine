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
    public class Scene : IHandleGameEvents, IScreenInformation, IGameplayContainer
    {
        private Dictionary<string, ISceneObject> objects;
        private List<GameEntity> entities;
        private SceneInfo info;
        private int frame = 0;

        public event Action GameThink;
        public event Action GameAct;
        public event Action GameReact;
        public event Action GameCleanup;
        public event Action End;

        private Scene(SceneInfo info)
        {
            objects = new Dictionary<string, ISceneObject>();
            entities = new List<GameEntity>();
            this.info = info;
        }

        public void StartHandler()
        {
            frame = 0;
            Engine.Instance.GameLogicTick += Tick;
            Engine.Instance.GameRender += GameRender;
        }

        public void StopHandler()
        {
            Engine.Instance.GameLogicTick -= Tick;
            Engine.Instance.GameRender -= GameRender;
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
            foreach (var keyframe in info.KeyFrames)
            {
                if (keyframe.Frame == frame)
                {
                    if (keyframe.Fade)
                    {
                        KeyFrameInfo frameInfo = keyframe; // for closure
                        Engine.Instance.FadeTransition(() => TriggerKeyFrame(frameInfo));
                    }
                    else
                    {
                        TriggerKeyFrame(keyframe);
                    }
                }
            }

            if (!Game.CurrentGame.Paused)
            {
                if (GameThink != null) GameThink();
                if (GameAct != null) GameAct();
                if (GameReact != null) GameReact();
            }
            if (GameCleanup != null) GameCleanup();

            frame++;

            if (frame >= info.Duration && End != null)
            {
                End();
            }
        }

        private void TriggerKeyFrame(KeyFrameInfo info)
        {
            foreach (var cmd in info.Commands)
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

        private interface ISceneObject
        {
            void Start();
            void Stop();
            void Draw(GameGraphicsLayers layers, Color opacity);
        }

        private class SceneSprite : ISceneObject
        {
            private Sprite sprite;
            private Point location;

            public SceneSprite(Sprite sprite, Point location)
            {
                this.sprite = sprite;
                this.sprite.SetTexture(Engine.Instance.GraphicsDevice, this.sprite.SheetPath.Absolute);
                this.location = location;
                this.sprite.Play();
            }

            public void Start()
            {
                Engine.Instance.GameLogicTick += Update;
            }

            public void Stop()
            {
                Engine.Instance.GameLogicTick -= Update;
            }

            private void Update(GameTickEventArgs e)
            {
                sprite.Update();
            }

            public void Draw(GameGraphicsLayers layers, Color opacity)
            {
                sprite.DrawXna(layers.SpritesBatch[sprite.Layer], opacity, location.X, location.Y);
            }
        }

        private class SceneText : ISceneObject
        {
            private string content;
            private string displayed = "";
            private int speed;
            private int frame;
            private Vector2 position;

            public SceneText(string content, int? speed, int x, int y)
            {
                this.content = content;
                this.speed = speed ?? 0;
                this.position = new Vector2(x, y);
            }

            public void Start()
            {
                if (speed == 0)
                {
                    displayed = content;
                }
                else
                {
                    Engine.Instance.GameLogicTick += Update;
                    frame = 0;
                }
            }

            public void Stop()
            {
                if (speed != 0)
                {
                    Engine.Instance.GameLogicTick -= Update;
                }
            }

            private void Update(GameTickEventArgs e)
            {
                frame++;
                if (frame >= speed && displayed.Length < content.Length)
                {
                    // add a character to the displayed text
                    displayed += content.Substring(displayed.Length, 1);
                    frame = 0;
                }
            }

            public void Draw(GameGraphicsLayers layers, Color opacity)
            {
                FontSystem.Draw(layers.ForegroundBatch, "Big", displayed, position);
            }
        }

        private class SceneFill : ISceneObject
        {
            private Texture2D texture;
            private float x, y, width, height;
            private float vx, vy, vw, vh, duration;
            private int stopX, stopY, stopWidth, stopHeight, moveFrame;
            private int layer;

            public SceneFill(Color color, int x, int y, int width, int height, int layer)
            {
                this.texture = new Texture2D(Engine.Instance.GraphicsDevice, 1, 1);
                this.texture.SetData(new Color[] { color });
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
                this.layer = layer;
            }

            public void Start() { }

            public void Stop() { }

            public void Draw(GameGraphicsLayers layers, Color opacity)
            {
                layers.SpritesBatch[layer].Draw(texture, new Rectangle((int)x, (int)y, (int)width, (int)height), opacity);
            }

            public void Move(int nx, int ny, int nwidth, int nheight, int duration)
            {
                this.stopX = nx;
                this.stopY = ny;
                this.stopWidth = nwidth;
                this.stopHeight = nheight;
                this.duration = duration;
                vx = (nx - x) / duration;
                vy = (ny - y) / duration;
                vw = (nwidth - width) / duration;
                vh = (nheight - height) / duration;
                moveFrame = 0;

                Engine.Instance.GameLogicTick += Update;
            }

            private void Update(GameTickEventArgs e)
            {
                x += vx;
                y += vy;
                width += vw;
                height += vh;
                moveFrame++;

                if (moveFrame >= duration)
                {
                    x = stopX;
                    y = stopY;
                    width = stopWidth;
                    height = stopHeight;
                    Engine.Instance.GameLogicTick -= Update;
                }
            }
        }

        private static Dictionary<string, Scene> scenes = new Dictionary<string,Scene>();

        public static void LoadScene(XElement node)
        {
            var info = SceneInfo.FromXml(node, Game.CurrentGame.BasePath);
            scenes.Add(info.Name, new Scene(info));
        }

        public static Scene Get(string name)
        {
            return scenes[name];
        }

        public static void Unload()
        {
            scenes.Clear();
        }
    }
}
