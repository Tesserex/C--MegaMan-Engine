using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Editor.Bll.Tools
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

        event EventHandler<ToolChangedEventArgs> ToolChanged;
    }
}
