using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;
using Microsoft.Xna.Framework;
using System.Xml.Linq;

namespace MegaMan.Engine
{
    public class Scene : IHandleGameEvents, IScreenInformation
    {
        private Dictionary<string, SceneObject> objects;
        private List<GameEntity> entities;
        private SceneInfo info;
        private int frame = 0;

        public event Action Finished;

        private Scene(SceneInfo info)
        {
            objects = new Dictionary<string, SceneObject>();
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

            frame++;

            if (frame >= info.Duration && Finished != null)
            {
                Finished();
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

                    case KeyFrameCommands.Add:
                        AddCommand((KeyFrameAddCommandInfo)cmd);
                        break;

                    case KeyFrameCommands.Remove:
                        RemoveCommand((KeyFrameRemoveCommandInfo)cmd);
                        break;

                    case KeyFrameCommands.Entity:
                        EntityCommand((KeyFrameEntityCommandInfo)cmd);
                        break;
                }
            }
        }

        private void PlayMusicCommand(KeyFramePlayCommandInfo command)
        {
            Engine.Instance.SoundSystem.PlayMusicNSF((uint)command.Track);
        }

        private void AddCommand(KeyFrameAddCommandInfo command)
        {
            var obj = new SceneObject(info.Sprites[command.Sprite], new Point(command.X, command.Y));
            obj.Start();
            objects.Add(command.Name, obj);
        }

        private void RemoveCommand(KeyFrameRemoveCommandInfo command)
        {
            objects[command.Name].Stop();
            objects.Remove(command.Name);
        }

        private void EntityCommand(KeyFrameEntityCommandInfo command)
        {
            var entity = GameEntity.Get(command.Name);
            entity.Screen = this;
            entity.GetComponent<PositionComponent>().SetPosition(command.X, command.Y);
            if (!string.IsNullOrEmpty(command.State))
            {
                entity.SendMessage(new StateMessage(null, command.State));
            }
            entities.Add(entity);
            entity.Start();
        }

        private class SceneObject
        {
            private Sprite sprite;
            private Point location;

            public SceneObject(Sprite sprite, Point location)
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
