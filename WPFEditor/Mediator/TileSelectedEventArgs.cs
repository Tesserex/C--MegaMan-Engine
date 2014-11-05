using System;
using MegaMan.Common;

namespace MegaMan.Editor.Mediator
{
    public class TileSelectedEventArgs : EventArgs
    {
        public Tile Tile { get; set; }
    }
}
