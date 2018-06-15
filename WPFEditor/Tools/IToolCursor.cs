using System;
using System.Windows;

namespace MegaMan.Editor.Tools
{
    public interface IToolCursor : IDisposable
    {
        void ApplyCursorTo(FrameworkElement element);
    }
}
