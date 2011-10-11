using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;
using Microsoft.Xna.Framework;
using System.Xml.Linq;

namespace MegaMan.Engine
{
    public class Scene : IHandleGameEvents
    {
        private Dictionary<string, SceneObject> objects;
        private SceneInfo info;
        private int frame = 0;

        public event Action Finished;

        private Scene(SceneInfo info)
        {
            objects = new Dictionary<string, SceneObject>();
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
            objects.Add(command.Name, obj);
        }

        private void RemoveCommand(KeyFrameRemoveCommandInfo command)
        {
            objects.Remove(command.Name);
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
            }

            public void Draw(GameGraphicsLayers layers, Color opacity)
            {
                sprite.DrawXna(layers.BackgroundBatch, opacity, location.X, location.Y);
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
