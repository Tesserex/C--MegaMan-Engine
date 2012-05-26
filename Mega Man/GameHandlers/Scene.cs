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
    public class Scene : GameHandler
    {
        private SceneInfo info;
        private int frame = 0;

        private Scene(SceneInfo info)
        {
            objects = new Dictionary<string, IHandlerObject>();
            this.info = info;
            Info = info;
        }

        public override void StartHandler()
        {
            frame = 0;
            base.StartHandler();
        }

        protected override void GameInputReceived(GameInputEventArgs e)
        {
            if (info.CanSkip && e.Pressed && e.Input == GameInput.Start)
            {
                Finish(info.NextHandler);
            }
        }

        protected override void Tick(GameTickEventArgs e)
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

            base.Tick(e);

            frame++;

            if (frame >= info.Duration)
            {
                Finish(info.NextHandler);
            }
        }

        private void TriggerKeyFrame(KeyFrameInfo info)
        {
            RunCommands(info.Commands);
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
}
