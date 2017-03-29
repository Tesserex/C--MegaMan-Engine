using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MegaMan.Engine.Forms.Settings;

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
            Set(p == this.state);
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            this.controller.Raise(this.state);
        }
        public void LoadSettings(Setting settings)
        {
            if (settings.Screens.Pixellated == this.state)
                this.controller.Raise(this.state);
        }

        public void SaveSettings(Setting settings)
        {
            if (this.menuItem.Checked)
            {
                if (settings.Screens == null)
                    settings.Screens = new LastScreen();

                settings.Screens.Pixellated = this.state;
            }
        }

        public void Set(bool value)
        {
            this.menuItem.Checked = value;
            if (value)
            {
                if (this.state == PixellatedOrSmoothed.Pixellated)
                {
                    Engine.Instance.FilterState = Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp;
                }
                else if (this.state == PixellatedOrSmoothed.Smoothed)
                {
                    Engine.Instance.FilterState = Microsoft.Xna.Framework.Graphics.SamplerState.LinearClamp;
                }
            }
        }
    }
}
