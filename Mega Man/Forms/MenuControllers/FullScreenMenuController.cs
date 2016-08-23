using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MegaMan.Engine.Forms.Settings;

namespace MegaMan.Engine.Forms.MenuControllers
{
    public class FullScreenMenuController : IMenuController
    {
        private readonly ScreenScaleController controller;
        private readonly ToolStripMenuItem item;
        private ScreenScale lastScale;

        public FullScreenMenuController(ScreenScaleController controller, ToolStripMenuItem item)
        {
            this.controller = controller;
            this.item = item;

            item.Click += Item_Click;
            controller.SizeChanged += Controller_SizeChanged;
            controller.NtscSet += Controller_NtscSet;
        }

        private void Controller_NtscSet(object sender, ScreenScaleNtscEventArgs e)
        {
            this.item.Checked = false;
            this.lastScale = ScreenScale.NTSC;
        }

        private void Controller_SizeChanged(object sender, ScreenScaleChangedEventArgs e)
        {
            if (e.Scale == ScreenScale.Fullscreen)
            {
                this.item.Checked = true;
            }
            else
            {
                this.lastScale = e.Scale;
                this.item.Checked = false;
            }
        }

        private void Item_Click(object sender, EventArgs e)
        {
            Set(!this.item.Checked);
        }

        public void LoadSettings(Setting settings)
        {
            if (settings.Screens.Size == ScreenScale.Fullscreen)
            {
                this.item.Checked = true;
                this.controller.Change(ScreenScale.Fullscreen);
            }
            else
            {
                this.item.Checked = false;
                // don't raise a size change, it would be wrong
            }
        }

        public void Set(bool value)
        {
            this.item.Checked = value;
            if (value)
            {
                this.controller.Change(ScreenScale.Fullscreen);
            }
            else
            {
                if (this.lastScale == ScreenScale.NTSC)
                    this.controller.Ntsc(null);
                else
                    this.controller.Change(this.lastScale);
            }
        }
    }
}
