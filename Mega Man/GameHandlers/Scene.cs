﻿using System;
using System.Collections.Generic;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    public class Scene : GameHandler
    {
        private SceneInfo info;
        private int frame = 0;
        private bool waiting = false;

        private ITiledScreen _screen;
        public override ITiledScreen Screen { get { return _screen; } }

        private Scene(SceneInfo info)
        {
            this.info = info;
            Info = info;
            _screen = new NullTiledScreen();
        }

        public override void StartHandler(IEntityPool entityPool)
        {
            frame = 0;
            base.StartHandler(entityPool);
        }

        protected override void RunCommand(SceneCommandInfo cmd)
        {
            if (cmd.Type == SceneCommands.WaitForInput)
            {
                waiting = true;
            }
            else
            {
                base.RunCommand(cmd);
            }
        }

        protected override void GameInputReceived(GameInputEventArgs e)
        {
            if (waiting)
            {
                if (e.Input == GameInput.Shoot || e.Input == GameInput.Jump || e.Input == GameInput.Start)
                {
                    waiting = false;
                }
            }
            else if (info.CanSkip && e.Pressed && e.Input == GameInput.Start)
            {
                Finish(info.NextHandler);
            }
        }

        protected override void Tick(GameTickEventArgs e)
        {
            if (waiting)
            {
                base.Tick(e);
                return;
            }

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

        public static void LoadScenes(IEnumerable<SceneInfo> scenesInfo)
        {
            foreach (var info in scenesInfo)
            {
                if (scenes.ContainsKey(info.Name)) throw new GameRunException(String.Format("You have two Scenes with the name of {0} - names must be unique.", info.Name));

                scenes.Add(info.Name, new Scene(info));
            }
        }

        public static Scene Get(string name)
        {
            if (!scenes.ContainsKey(name))
            {
                throw new GameRunException(
                    String.Format("I tried to run the scene named '{0}', but couldn't find it.\nPerhaps it's not being included in the main file.", name)
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
