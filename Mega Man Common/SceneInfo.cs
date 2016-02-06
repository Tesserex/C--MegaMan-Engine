using System.Collections.Generic;

namespace MegaMan.Common
{
    public class SceneInfo : HandlerInfo
    {
        public int Duration { get; set; }
        public bool CanSkip { get; set; }

        public List<KeyFrameInfo> KeyFrames { get; private set; }
        public HandlerTransfer NextHandler { get; set; }

        public SceneInfo()
        {
            KeyFrames = new List<KeyFrameInfo>();
        }
    }

    public class KeyFrameInfo
    {
        public int Frame { get; set; }
        public bool Fade { get; set; }
        public List<SceneCommandInfo> Commands { get; set; }
    }
}
