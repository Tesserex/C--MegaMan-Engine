using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.Editor.Mediator
{
    public class NewEntityEventArgs : EventArgs
    {
        public string Name { get; set; }
    }
}
