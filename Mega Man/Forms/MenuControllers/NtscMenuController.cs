﻿using System.Windows.Forms;
using MegaMan.Engine.Forms.Settings;

namespace MegaMan.Engine.Forms.MenuControllers
{
    public class NtscMenuController : ScreenSizeMenuController
    {
        private readonly NTSC_Options ntscOption;
        private readonly snes_ntsc_setup_t ntscSetup;

        public NtscMenuController(ScreenScaleController controller, ToolStripMenuItem item, NTSC_Options ntscOption) : base(controller, item, ScreenScale.NTSC)
        {
            this.ntscOption = ntscOption;
            switch (ntscOption)
            {
                case NTSC_Options.Composite:
                    ntscSetup = snes_ntsc_setup_t.snes_ntsc_composite;
                    break;

                case NTSC_Options.RGB:
                    ntscSetup = snes_ntsc_setup_t.snes_ntsc_rgb;
                    break;

                case NTSC_Options.S_Video:
                    ntscSetup = snes_ntsc_setup_t.snes_ntsc_svideo;
                    break;
            }
        }

        protected override void Controller_NtscSet(object sender, ScreenScaleNtscEventArgs e)
        {
            item.Checked = (e.Setup == ntscSetup);
        }

        protected override void Controller_SizeChanged(object sender, ScreenScaleChangedEventArgs e)
        {
            // noop
        }

        public override void Set(bool value)
        {
            item.Checked = value;
            if (value)
            {
                controller.Ntsc(ntscSetup);
            }
        }

        public override void LoadSettings(Setting settings)
        {
            Set(settings.Screens.Size == ScreenScale.NTSC && settings.Screens.NTSC_Options == ntscOption);
        }

        public override void SaveSettings(Setting settings)
        {
            if (settings.Screens == null)
                settings.Screens = new LastScreen();

            if (item.Checked)
            {
                settings.Screens.Size = ScreenScale.NTSC;
                settings.Screens.NTSC_Options = ntscOption;
            }
        }
    }
}
