using MegaMan.Editor.Bll.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

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
