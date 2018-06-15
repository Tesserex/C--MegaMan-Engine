using System;
using MegaMan.Common;

namespace MegaMan.Editor.Mediator
{
    public class StageAddedEventArgs : EventArgs
    {
        public StageLinkInfo Stage { get; set; }
    }
}
