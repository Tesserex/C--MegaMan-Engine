using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace MegaMan.Editor.Tools
{
    public interface IToolCursor
    {
        ImageSource CursorImage { get; }

        Double CursorWidth { get; }

        Double CursorHeight { get; }
    }
}
