
using System;

namespace MegaMan.Common
{
    public enum HandlerType
    {
        Stage,
        Scene,
        Menu
    }

    public enum HandlerMode
    {
        Next,
        Push,
        Pop
    }

    public class HandlerTransfer
    {
        public HandlerType Type;
        public HandlerMode Mode;
        public string Name;
        public bool Fade;
        public bool Pause;

        public HandlerTransfer Clone()
        {
            return new HandlerTransfer() {
                Type = this.Type,
                Mode = this.Mode,
                Name = this.Name,
                Fade = this.Fade,
                Pause = this.Pause
            };
        }
    }
}
