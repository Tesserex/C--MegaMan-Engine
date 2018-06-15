
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
            return new HandlerTransfer {
                Type = Type,
                Mode = Mode,
                Name = Name,
                Fade = Fade,
                Pause = Pause
            };
        }
    }
}
