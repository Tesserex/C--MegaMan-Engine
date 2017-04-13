using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common.Entities;

namespace MegaMan.Editor.Mediator
{
    public class EntityAddedEventArgs : EventArgs
    {
        public EntityInfo Entity { get; set; }
    }
}
