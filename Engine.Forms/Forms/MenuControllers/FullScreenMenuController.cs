using System;
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
            item.Checked = false;
            lastScale = ScreenScale.NTSC;
        }

        private void Controller_SizeChanged(object sender, ScreenScaleChangedEventArgs e)
        {
            if (e.Scale == ScreenScale.Fullscreen)
            {
                item.Checked = true;
            }
            else
            {
                lastScale = e.Scale;
                item.Checked = false;
            }
        }

        private void Item_Click(object sender, EventArgs e)
        {
            Set(!item.Checked);
        }

        public void LoadSettings(Setting settings)
        {
            if (settings.Screens.Size == ScreenScale.Fullscreen)
            {
                item.Checked = true;
                controller.Change(ScreenScale.Fullscreen);
            }
            else
            {
                item.Checked = false;
                // don't raise a size change, it would be wrong
            }
        }

        public void Set(bool value)
        {
            item.Checked = value;
            if (value)
            {
                controller.Change(ScreenScale.Fullscreen);
            }
            else
            {
                if (lastScale == ScreenScale.NTSC)
                    controller.Ntsc(null);
                else
                    controller.Change(lastScale);
            }
        }

        public void SaveSettings(Setting settings)
        {
            if (settings.Screens == null)
                settings.Screens = new LastScreen();

            if (item.Checked)
                settings.Screens.Size = ScreenScale.Fullscreen;
        }
    }
}
