using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MegaMan.Engine.Forms.Settings;

namespace MegaMan.Engine.Forms.MenuControllers
{
    public class ScreenSizeMenuController : IMenuController
    {
        protected ScreenScaleController controller;
        protected ScreenScale scale;
        protected ToolStripMenuItem item;

        public ScreenSizeMenuController(ScreenScaleController controller, ToolStripMenuItem item, ScreenScale scale)
        {
            this.controller = controller;
            this.scale = scale;
            this.item = item;

            item.Click += Item_Click;
            controller.SizeChanged += Controller_SizeChanged;
            controller.NtscSet += Controller_NtscSet;
        }

        protected virtual void Controller_NtscSet(object sender, ScreenScaleNtscEventArgs e)
        {
            this.item.Checked = false;
        }

        protected virtual void Controller_SizeChanged(object sender, ScreenScaleChangedEventArgs e)
        {
            this.item.Checked = (e.Scale == this.scale);
        }

        private void Item_Click(object sender, EventArgs e)
        {
            Set(!item.Checked);
        }

        public void LoadSettings(Setting settings)
        {
            Set(settings.Screens.Size == this.scale);
        }

        public virtual void Set(bool value)
        {
            this.item.Checked = value;
            if (value)
            {
                this.controller.Change(this.scale);
            }
        }
    }
}
