using System;
using System.Windows.Forms;

namespace MegaMan.Engine
{
    [Serializable]
    public class UserSettings
    {
        public UserKeys Keys { get; set; }
    }

    [Serializable]
    public class UserKeys
    {
        public Keys Up { get; set; }
        public Keys Down { get; set; }
        public Keys Left { get; set; }
        public Keys Right { get; set; }
        public Keys Jump { get; set; }
        public Keys Shoot { get; set; }
        public Keys Start { get; set; }
        public Keys Select { get; set; }
    }
}
