using System;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Mediator
{
    public class StageChangedEventArgs : EventArgs
    {
        public StageDocument Stage { get; private set; }

        public StageChangedEventArgs(StageDocument stage)
        {
            Stage = stage;
        }
    }
}
