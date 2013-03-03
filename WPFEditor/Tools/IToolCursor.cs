using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MegaMan.Editor.Tools
{
    public interface IToolCursor : IDisposable
    {
        void ApplyCursorTo(FrameworkElement element);
    }
}
