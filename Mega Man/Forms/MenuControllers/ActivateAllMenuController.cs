using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MegaMan.Engine.Forms.Settings;

namespace MegaMan.Engine.Forms.MenuControllers
{
    public class ActivateAllMenuController : IMenuController
    {
        private readonly ToolStripMenuItem menuItem;
        private readonly List<IMenuController> controllers;

        public ActivateAllMenuController(ToolStripMenuItem menuItem, params IMenuController[] controllers)
        {
            this.menuItem = menuItem;
            this.controllers = controllers.ToList();

            menuItem.Click += MenuItem_Click;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            Set(true);
        }

        public void Set(bool value)
        {
            foreach (var c in controllers)
                c.Set(value);
        }

        public void LoadSettings(Setting settings)
        {
            
        }

        public void SaveSettings(Setting settings)
        {
            
        }
    }
}
