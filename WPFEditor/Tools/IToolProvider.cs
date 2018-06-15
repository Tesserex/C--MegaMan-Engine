using System;
using MegaMan.Editor.Bll.Tools;

namespace MegaMan.Editor.Tools
{
    public class ToolChangedEventArgs : EventArgs
    {
        public IToolBehavior Tool { get; private set; }

        public ToolChangedEventArgs(IToolBehavior tool)
        {
            Tool = tool;
        }
    }

    public interface IToolProvider
    {
        IToolBehavior Tool { get; }

        IToolCursor ToolCursor { get; }

        event EventHandler<ToolChangedEventArgs> ToolChanged;
    }
}
