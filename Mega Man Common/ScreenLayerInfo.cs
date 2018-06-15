using System;
using System.Collections.Generic;
using System.Linq;

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
            Name = name;
            Tiles = tiles;
            Foreground = foreground;
            Entities = new List<EntityPlacement>();
            Keyframes = keyframes;
        }

        public void AddEntity(EntityPlacement entity)
        {
            Entities.Add(entity);
        }

        public ScreenLayerInfo Clone()
        {
            return new ScreenLayerInfo(Name, Tiles.Clone(), Foreground, Keyframes.Select(x => x.Clone()).ToList()) {
                Entities = Entities.Select(x => x.Clone()).ToList()
            };
        }
    }

    public class ScreenLayerKeyframe
    {
        public int Frame { get; set; }
        public ScreenLayerMoveCommand Move { get; set; }
        public bool Reset { get; set; }

        public ScreenLayerKeyframe Clone()
        {
            return new ScreenLayerKeyframe {
                Frame = Frame,
                Move = Move.Clone(),
                Reset = Reset
            };
        }
    }

    public class ScreenLayerMoveCommand
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Duration { get; set; }

        public ScreenLayerMoveCommand Clone()
        {
            return new ScreenLayerMoveCommand {
                X = X,
                Y = Y,
                Duration = Duration
            };
        }
    }
}
