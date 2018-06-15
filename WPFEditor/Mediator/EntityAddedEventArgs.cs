using System;
using MegaMan.Common.Entities;

namespace MegaMan.Editor.Mediator
{
    public class EntityAddedEventArgs : EventArgs
    {
        public EntityInfo Entity { get; set; }
    }
}
