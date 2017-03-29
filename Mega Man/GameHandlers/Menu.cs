﻿using System;
using System.Collections.Generic;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Common.IncludedObjects;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    public class Menu : GameHandler
    {
        private MenuInfo info;
        private MenuStateInfo state;
        private List<MenuOptionCommandInfo> options;
        private int selectedId;
        private Point currentPos;

        private ITiledScreen _screen;
        public override ITiledScreen Screen { get { return _screen; } }

        private Menu(MenuInfo info)
        {
            this.info = info;
            Info = info;
            this.options = new List<MenuOptionCommandInfo>();
            _screen = new NullTiledScreen();
        }

        private void ResetState()
        {
            this.selectedId = 0;

            if (this.state.StartOptionName != null)
            {
                var findId = this.options.FindIndex(o => o.Name == this.state.StartOptionName);
                if (findId >= 0)
                {
                    this.selectedId = findId;
                }
            }
            else if (this.state.StartOptionVar != null)
            {
                var findId = this.options.FindIndex(o => o.Name == Game.CurrentGame.Player.Var(this.state.StartOptionVar));
                if (findId >= 0)
                {
                    this.selectedId = findId;
                }
            }

            var option = this.options[this.selectedId];

            if (option.OnEvent != null)
            {
                RunCommands(option.OnEvent);
            }

            this.currentPos = new Point(option.X, option.Y);
        }

        public override void StartHandler(IEntityPool entityPool)
        {
            base.StartHandler(entityPool);

            this.state = this.info.States[0];
            RunCommands(this.state.Commands);

            ResetState();
        }

        protected override void RunCommands(IEnumerable<SceneCommandInfo> commands)
        {
            base.RunCommands(commands);

            foreach (var cmd in commands)
            {
                if (cmd.Type == SceneCommands.Option)
                {
                    this.options.Add((MenuOptionCommandInfo)cmd);
                }
            }
        }

        protected override void GameInputReceived(GameInputEventArgs e)
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

        private static Dictionary<string, Menu> menus = new Dictionary<string, Menu>();

        public static void LoadMenus(IEnumerable<MenuInfo> menusInfo)
        {
            foreach (var info in menusInfo)
            {
                if (menus.ContainsKey(info.Name)) throw new GameRunException(String.Format("You have two Menus with the name of {0} - names must be unique.", info.Name));

                menus.Add(info.Name, new Menu(info));
            }
        }

        public static Menu Get(string name)
        {
            if (!menus.ContainsKey(name))
            {
                throw new GameRunException(
                    String.Format("I tried to run the menu named '{0}', but couldn't find it.\nPerhaps it's not being included in the main file.", name)
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
