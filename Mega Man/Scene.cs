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
    public class Scene : IScreenInformation, IGameplayContainer
    {
        private Dictionary<string, ISceneObject> objects;
        private List<GameEntity> entities;
        private SceneInfo info;
        private int frame = 0;

        public event Action GameThink;
        public event Action GameAct;
        public event Action GameReact;
        public event Action GameCleanup;

        public event Action<HandlerTransfer> End;

        private Scene(SceneInfo info)
        {
            objects = new Dictionary<string, ISceneObject>();
            entities = new List<GameEntity>();
            this.info = info;
        }

        public void StartHandler()
        {
            frame = 0;
            ResumeHandler();
            StartDrawing();
        }

        private bool running;

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

        public void StopDrawing() 
        {
            Engine.Instance.GameRender -= GameRender;
        }

        public void StartDrawing()
        {
            Engine.Instance.GameRender += GameRender;
        }

        public void StopHandler()
        {
            PauseHandler();
            StopDrawing();

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
            get;
            set;
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
            if (info.CanSkip && e.Pressed && e.Input == GameInput.Start && End != null)
            {
                End(info.NextHandler);
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

            if (GameThink != null) GameThink();
            if (GameAct != null) GameAct();
            if (GameReact != null) GameReact();
            if (GameCleanup != null) GameCleanup();

            frame++;

            if (frame >= info.Duration && End != null)
            {
                End(info.NextHandler);
            }
        }

        private void TriggerKeyFrame(KeyFrameInfo info)
        {
            foreach (var cmd in info.Commands)
            {
                switch (cmd.Type)
                {
                    case SceneCommands.PlayMusic:
                        PlayMusicCommand((ScenePlayCommandInfo)cmd);
                        break;

                    case SceneCommands.Add:
                        AddCommand((SceneAddCommandInfo)cmd);
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
                }
            }
        }

        private void PlayMusicCommand(ScenePlayCommandInfo command)
        {
            Engine.Instance.SoundSystem.PlayMusicNSF((uint)command.Track);
        }

        private void AddCommand(SceneAddCommandInfo command)
        {
            var obj = info.Objects[command.Object];

            ISceneObject handler = null;
            if (obj is HandlerSprite)
            {
                handler = new SceneSprite(((HandlerSprite)obj).Sprite, new Point(command.X, command.Y));
            }
            else if (obj is MeterInfo)
            {
                handler = new SceneMeter(HealthMeter.Create((MeterInfo)obj, false), this);
            }
            handler.Start();
            var name = command.Name ?? Guid.NewGuid().ToString();
            if (!objects.ContainsKey(name)) objects.Add(name, handler);
        }

        private void TextCommand(SceneTextCommandInfo command)
        {
            var obj = new SceneText(command, this);
            obj.Start();
            var name = command.Name ?? Guid.NewGuid().ToString();
            if (!objects.ContainsKey(name)) objects.Add(name, obj);
        }

        private void RemoveCommand(SceneRemoveCommandInfo command)
        {
            if (!objects.ContainsKey(command.Name))
            {
                throw new GameRunException(String.Format("The scene '{0}' referenced an object called '{1}', which doesn't exist.", this.info.Name, command.Name));
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
            entities.Add(entity);
            entity.Start(this);
        }

        private void FillCommand(SceneFillCommandInfo command)
        {
            Color color = new Color(command.Red, command.Green, command.Blue);

            var obj = new SceneFill(color, command.X, command.Y, command.Width, command.Height, command.Layer);
            obj.Start();
            var name = command.Name ?? Guid.NewGuid().ToString();
            if (!objects.ContainsKey(name)) objects.Add(name, obj);
        }

        private void FillMoveCommand(SceneFillMoveCommandInfo command)
        {
            SceneFill obj = objects[command.Name] as SceneFill;
            obj.Move(command.X, command.Y, command.Width, command.Height, command.Duration);
        }

        private static Dictionary<string, Scene> scenes = new Dictionary<string,Scene>();

        public static void LoadScene(XElement node)
        {
            var info = SceneInfo.FromXml(node, Game.CurrentGame.BasePath);

            if (scenes.ContainsKey(info.Name)) throw new GameXmlException(node, String.Format("You have two Scenes with the name of {0} - names must be unique.", info.Name));

            scenes.Add(info.Name, new Scene(info));
        }

        public static Scene Get(string name)
        {
            if (!scenes.ContainsKey(name))
            {
                throw new GameRunException(
                    String.Format("I tried to run the menu named '{0}', but couldn't find it.\nPerhaps it's not being included in the main file.", name)
                );
            }

            return scenes[name];
        }

        public static void Unload()
        {
            scenes.Clear();
        }
    }

    public interface ISceneObject
    {
        void Start();
        void Stop();
        void Draw(GameGraphicsLayers layers, Color opacity);
    }

    public class SceneSprite : ISceneObject
    {
        private Sprite sprite;

        private float x, y;
        private float vx, vy, duration;
        private int stopX, stopY, moveFrame;

        public SceneSprite(Sprite sprite, Point location)
        {
            this.sprite = new Sprite(sprite);
            this.sprite.SetTexture(Engine.Instance.GraphicsDevice, this.sprite.SheetPath.Absolute);
            this.x = location.X;
            this.y = location.Y;
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

        public void Reset()
        {
            sprite.Reset();
        }

        private void Update(GameTickEventArgs e)
        {
            sprite.Update();
        }

        public void Move(int nx, int ny, int duration)
        {
            this.stopX = nx;
            this.stopY = ny;
            this.duration = duration;
            vx = (nx - x) / duration;
            vy = (ny - y) / duration;
            moveFrame = 0;

            Engine.Instance.GameLogicTick += MoveUpdate;
        }

        private void MoveUpdate(GameTickEventArgs e)
        {
            x += vx;
            y += vy;
            moveFrame++;

            if (moveFrame >= duration)
            {
                x = stopX;
                y = stopY;
                Engine.Instance.GameLogicTick -= MoveUpdate;
            }
        }

        public void Draw(GameGraphicsLayers layers, Color opacity)
        {
            sprite.DrawXna(layers.SpritesBatch[sprite.Layer], opacity, x, y);
        }
    }

    public class SceneText : ISceneObject
    {
        public string Content { get; set; }

        private string displayed = "";
        private int speed;
        private int frame;
        private Vector2 position;
        private Binding binding;

        public SceneText(SceneTextCommandInfo info, IGameplayContainer scene)
        {
            this.Content = info.Content ?? String.Empty;
            this.speed = info.Speed ?? 0;
            this.position = new Vector2(info.X, info.Y);

            if (info.Binding != null)
            {
                this.binding = Binding.Create(info.Binding, this, scene);
            }
        }

        public void Start()
        {
            if (this.binding != null)
            {
                this.binding.Start();
            }

            if (speed == 0)
            {
                displayed = Content;
            }
            else
            {
                Engine.Instance.GameLogicTick += Update;
                frame = 0;
            }
        }

        public void Stop()
        {
            if (this.binding != null)
            {
                this.binding.Stop();
            }

            if (speed != 0)
            {
                Engine.Instance.GameLogicTick -= Update;
            }
        }

        private void Update(GameTickEventArgs e)
        {
            frame++;
            if (frame >= speed && displayed.Length < Content.Length)
            {
                // add a character to the displayed text
                displayed += Content.Substring(displayed.Length, 1);
                frame = 0;
            }
        }

        public void Draw(GameGraphicsLayers layers, Color opacity)
        {
            FontSystem.Draw(layers.ForegroundBatch, "Big", displayed, position);
        }
    }

    public class SceneFill : ISceneObject
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

    public class SceneMeter : ISceneObject
    {
        private IGameplayContainer container;
        public HealthMeter Meter { get; set; }

        public SceneMeter(HealthMeter meter, IGameplayContainer container)
        {
            this.Meter = meter;
            this.container = container;
        }

        public void Start()
        {
            Meter.Start(container);
        }

        public void Stop()
        {
            Meter.Stop();
        }

        public void Draw(GameGraphicsLayers layers, Color opacity)
        {
            Meter.Draw(layers.SpritesBatch[3]);
        }
    }
}
