using System;
using System.Windows.Forms;
using MegaMan.Engine.Forms.Settings;
using Microsoft.Xna.Framework.Graphics;

namespace MegaMan.Engine.Forms.MenuControllers
{
    public class PixellatedMenuController : IMenuController
    {
        private readonly ToolStripMenuItem menuItem;
        private readonly PixellatedOrSmoothed state;
        private readonly ExclusiveController<PixellatedOrSmoothed> controller;

        public PixellatedMenuController(ToolStripMenuItem menuItem, PixellatedOrSmoothed state, ExclusiveController<PixellatedOrSmoothed> controller)
        {
            this.menuItem = menuItem;
            this.state = state;
            this.controller = controller;

            menuItem.Click += MenuItem_Click;
            controller.Changed += Controller_Changed;
        }

        private void Controller_Changed(PixellatedOrSmoothed p)
        {
            Set(p == state);
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            controller.Raise(state);
        }
        public void LoadSettings(Setting settings)
        {
            if (settings.Screens.Pixellated == state)
                controller.Raise(state);
        }

        public void SaveSettings(Setting settings)
        {
            if (menuItem.Checked)
            {
                if (settings.Screens == null)
                    settings.Screens = new LastScreen();

                settings.Screens.Pixellated = state;
            }
        }

        public void Set(bool value)
        {
            menuItem.Checked = value;
            if (value)
            {
                if (state == PixellatedOrSmoothed.Pixellated)
                {
                    Engine.Instance.FilterState = SamplerState.PointClamp;
                }
                else if (state == PixellatedOrSmoothed.Smoothed)
                {
                    Engine.Instance.FilterState = SamplerState.LinearClamp;
                }
            }
        }
    }
}
