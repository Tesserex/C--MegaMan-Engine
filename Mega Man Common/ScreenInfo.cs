using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using MegaMan.Common.Geometry;

namespace MegaMan.Common
{
    public struct TeleportInfo
    {
        public Point From;
        public Point To;
        public string TargetScreen;
    }
    
    public class ScreenInfo
    {
        #region Properties
        public List<SceneCommandInfo> Commands { get; set; }
        public List<ScreenLayerInfo> Layers { get; private set; }
        public List<BlockPatternInfo> BlockPatterns { get; private set; }
        public List<TeleportInfo> Teleports { get; private set; } 
        
        public string Name { get; set; }
        public int Width { get { return Layers[0].Tiles.Width; } }
        public int Height { get { return Layers[0].Tiles.Height; } }
        public int PixelWidth { get { return Layers[0].Tiles.PixelWidth; } }
        public int PixelHeight { get { return Layers[0].Tiles.PixelHeight; } }
        public Tileset Tileset { get; set; }

        #endregion Properties

        public ScreenInfo(string name, Tileset tileset)
        {
            this.Name = name;
            this.Tileset = tileset;

            BlockPatterns = new List<BlockPatternInfo>();
            Teleports = new List<TeleportInfo>();
            Layers = new List<ScreenLayerInfo>();
            Commands = new List<SceneCommandInfo>();
        }

        
    }
}
