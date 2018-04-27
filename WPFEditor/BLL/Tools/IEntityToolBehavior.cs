using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.Editor.Bll.Tools
{
    public interface IEntityToolBehavior : IToolBehavior
    {
        int SnapX { get; set; }
        int SnapY { get; set; }
    }
}
