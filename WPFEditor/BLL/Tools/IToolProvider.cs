using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Editor.Bll.Tools
{
    public interface IToolProvider
    {
        IToolBehavior Tool { get; }
    }
}
