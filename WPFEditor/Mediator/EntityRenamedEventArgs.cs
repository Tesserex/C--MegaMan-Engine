using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common.Entities;

namespace MegaMan.Editor.Mediator
{
    public class EntityRenamedEventArgs : EventArgs
    {
        public string OldName { get; set; }
        public EntityInfo Entity { get; set; }
    }
}
