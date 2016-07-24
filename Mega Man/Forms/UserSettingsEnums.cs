using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Forms
{
    class UserSettingsEnums
    {
        public enum Screen { X1, X2, X3, X4, NTSC }

        public enum NTSC_Options { Composite, S_Video, RGB, Custom }

        public enum PixellatedOrSmoothed { Pixellated, Smoothed }

        public enum Layers { Background, Sprite1, Sprite2, Sprite3, Sprite4, Foreground }
    }
}