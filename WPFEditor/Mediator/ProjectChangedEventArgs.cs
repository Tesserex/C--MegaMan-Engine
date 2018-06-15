using System;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Mediator
{
    public class ProjectChangedEventArgs : EventArgs
    {
        public ProjectDocument Project { get; set; }
    }
}
