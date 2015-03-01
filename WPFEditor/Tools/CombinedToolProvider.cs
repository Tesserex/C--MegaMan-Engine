using System;
using MegaMan.Editor.Bll.Tools;

namespace MegaMan.Editor.Tools
{
    internal class CombinedToolProvider : IToolProvider
    {
        private IToolProvider[] _providers;

        public CombinedToolProvider(params IToolProvider[] providers)
        {
            _providers = providers;

            foreach (var p in providers)
            {
                p.ToolChanged += p_ToolChanged;
            }
        }

        void p_ToolChanged(object sender, ToolChangedEventArgs e)
        {
            Tool = e.Tool;
            ToolCursor = ((IToolProvider)sender).ToolCursor;

            if (ToolChanged != null)
            {
                ToolChanged(this, e);
            }
        }

        public IToolBehavior Tool
        {
            get;
            private set;
        }

        public IToolCursor ToolCursor
        {
            get;
            private set;
        }

        public event EventHandler<ToolChangedEventArgs> ToolChanged;
    }
}
