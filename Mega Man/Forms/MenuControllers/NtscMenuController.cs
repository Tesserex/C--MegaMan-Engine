using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MegaMan.Engine.Forms.Settings;

namespace MegaMan.Engine.Forms.MenuControllers
{
    public class NtscMenuController : ScreenSizeMenuController
    {
        private readonly snes_ntsc_setup_t ntscSetup;

        public NtscMenuController(ScreenScaleController controller, ToolStripMenuItem item, snes_ntsc_setup_t ntscSetup) : base(controller, item, ScreenScale.NTSC)
        {
            this.ntscSetup = ntscSetup;
        }

        protected override void Controller_NtscSet(object sender, ScreenScaleNtscEventArgs e)
        {
            this.item.Checked = (e.Setup == this.ntscSetup);
        }

        protected override void Controller_SizeChanged(object sender, ScreenScaleChangedEventArgs e)
        {
            // noop
        }

        public override void Set(bool value)
        {
            this.item.Checked = value;
            if (value)
            {
                this.controller.Ntsc(this.ntscSetup);
            }
        }
    }
}
