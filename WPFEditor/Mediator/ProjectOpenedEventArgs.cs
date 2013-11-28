using MegaMan.Editor.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.Editor.Mediator
{
    public class ProjectOpenedEventArgs : EventArgs
    {
        public ProjectDocument Project { get; set; }
    }
}
