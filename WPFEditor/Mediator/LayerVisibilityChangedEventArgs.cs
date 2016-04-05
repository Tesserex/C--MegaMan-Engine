using System;

namespace MegaMan.Editor.Mediator
{
    public class LayerVisibilityChangedEventArgs : EventArgs
    {
        public bool BordersVisible { get; set; }
        public bool TilePropertiesVisible { get; set; }
    }
}
