using System;
using MegaMan.Common.Entities;

namespace MegaMan.Editor.Mediator {
    public class EntitySelectedEventArgs : EventArgs
    {
        public EntityInfo Entity { get; private set; }

        public EntitySelectedEventArgs(EntityInfo entity)
        {
            Entity = entity;
        }
    }
}
