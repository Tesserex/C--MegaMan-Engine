using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Engine.Forms.Settings;

namespace MegaMan.Engine.Forms
{
    public class ScreenScaleController
    {
        public event EventHandler<ScreenScaleChangedEventArgs> SizeChanged;
        public event EventHandler<ScreenScaleNtscEventArgs> NtscSet;

        public ScreenScale CurrentScale { get; private set; }

        public void Change(ScreenScale scale)
        {
            CurrentScale = scale;
            Raise(scale);
        }

        public void Ntsc(snes_ntsc_setup_t setup)
        {
            CurrentScale = ScreenScale.NTSC;
            var e = NtscSet;
            if (e != null)
            {
                var args = new ScreenScaleNtscEventArgs() { Setup = setup };
                e(this, args);
            }
        }

        private void Raise(ScreenScale scale)
        {
            var e = SizeChanged;
            if (e != null)
            {
                var args = new ScreenScaleChangedEventArgs() { Scale = scale };
                e(this, args);
            }
        }
    }

    public class ScreenScaleChangedEventArgs : EventArgs
    {
        public ScreenScale Scale { get; set; }
    }

    public class ScreenScaleNtscEventArgs : EventArgs
    {
        public snes_ntsc_setup_t Setup { get; set; }
    }
}
