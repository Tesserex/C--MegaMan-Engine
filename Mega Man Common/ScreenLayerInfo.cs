using System;
using System.Collections.Generic;

namespace MegaMan.Common
{
    public class ScreenLayerInfo
    {
        public String Name { get; private set; }
        public TileLayer Tiles { get; private set; }
        public Boolean Foreground { get; private set; }
        public Boolean Parallax { get; set; }
        public List<EntityPlacement> Entities { get; private set; }
        public List<ScreenLayerKeyframe> Keyframes { get; private set; }

        public ScreenLayerInfo(string name, TileLayer tiles, bool foreground, List<ScreenLayerKeyframe> keyframes)
        {
            this.Name = name;
            this.Tiles = tiles;
            this.Foreground = foreground;
            this.Entities = new List<EntityPlacement>();
            this.Keyframes = keyframes;
        }

        public void AddEntity(EntityPlacement entity)
        {
            this.Entities.Add(entity);
        }
    }

    public class ScreenLayerKeyframe
    {
        public int Frame { get; set; }
        public ScreenLayerMoveCommand Move { get; set; }
        public bool Reset { get; set; }
    }

    public class ScreenLayerMoveCommand
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Duration { get; set; }
    }
}
