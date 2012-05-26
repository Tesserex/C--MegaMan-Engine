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
    public class Menu : GameHandler
    {
        private MenuInfo info;
        private MenuStateInfo state;
        private List<MenuOptionCommandInfo> options;
        private int selectedId;
        private Point currentPos;

        private Menu(MenuInfo info)
        {
            objects = new Dictionary<string, IHandlerObject>();
            this.info = info;
            Info = info;
        }

        private void ResetState()
        {
            this.options = this.state.Commands.OfType<MenuOptionCommandInfo>().ToList();

            var option = this.options[0];
            this.selectedId = 0;
            this.currentPos = new Point(option.X, option.Y);
        }

        public override void StartHandler()
        {
            base.StartHandler();

            this.state = this.info.States[0];
            ResetState();
            RunCommands(this.state.Commands);
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
