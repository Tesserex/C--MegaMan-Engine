using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using MegaMan.Common.Geometry;

namespace MegaMan.Common
{
    public enum JoinType : int
    {
        Horizontal = 1,
        Vertical = 2
    }

    public enum JoinDirection : int
    {
        Both = 1,
        // <summary>
        // The player can only cross the join left to right, or top to bottom
        // </summary>
        ForwardOnly = 2,
        // <summary>
        // The player can only cross the join right to left, or bottom to top
        // </summary>
        BackwardOnly = 3
    }

    public class Join
    {
        public JoinType type;
        public string screenOne, screenTwo;
        // <summary>
        // The number of tiles from the top (if vertical) or left (if horizontal) of screenOne at which the join begins.
        // </summary>
        public int offsetOne;
        // <summary>
        // The number of tiles from the top (if vertical) or left (if horizontal) of screenTwo at which the join begins.
        // </summary>
        public int offsetTwo;
        // <summary>
        // The size extent of the join, in tiles.
        // </summary>
        public int Size;
        // <summary>
        // Whether the join allows the player to cross only one way or in either direction.
        // </summary>
        public JoinDirection direction;
        // <summary>
        // Whether this join has a boss-style door over it.
        // </summary>
        public bool bossDoor;
        public string bossEntityName;
    }

    public class StageInfo : HandlerInfo 
    {
        private Dictionary<string, Point> continuePoints;
        public IDictionary<string, Point> ContinuePoints { get { return continuePoints; } }

        private FilePath tilePath;

        #region Properties
        public Dictionary<string, ScreenInfo> Screens { get; private set; }
        public List<Join> Joins { get; private set; }
        public string StartScreen { get; set; }
        public int PlayerStartX { get; set; }
        public int PlayerStartY { get; set; }

        public Tileset Tileset { get; private set; }

        /// <summary>
        /// Gets or sets the absolute file path to the directory where this stage is stored
        /// </summary>
        public FilePath StagePath { get; set; }

        public FilePath MusicIntroPath { get; set; }
        public FilePath MusicLoopPath { get; set; }
        public int MusicNsfTrack { get; set; }
        
        #endregion Properties

        public StageInfo() 
        {
            Screens = new Dictionary<string, ScreenInfo>();
            Joins = new List<Join>();
            continuePoints = new Dictionary<string, Point>();
        }

        public void RenameScreen(ScreenInfo screen, string name)
        {
            this.Screens.Remove(screen.Name);
            screen.Name = name;
            this.Screens.Add(name, screen);
        }

        public void RenameScreen(string oldName, string newName)
        {
            RenameScreen(this.Screens[oldName], newName);
        }

        public void AddContinuePoint(string screenName, Point point)
        {
            continuePoints.Add(screenName, point);
        }

        /// <summary>
        /// Changes the tileset by specifying an absolute path to the new tileset XML file.
        /// </summary>
        /// <param name="path">If it's not absolute, I'll make it so.</param>
        public void ChangeTileset(string path)
        {
            tilePath = FilePath.FromAbsolute(path, StagePath.Absolute);
            Tileset = new Tileset(tilePath);
            
            foreach (ScreenInfo s in Screens.Values) s.Tileset = Tileset;
        }

        public void Clear() 
        {
            Screens.Clear();
            Joins.Clear();
            Tileset = null;
        }
    }
}
