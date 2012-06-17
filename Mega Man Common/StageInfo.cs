using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Xml.Linq;
using System.IO;

namespace MegaMan.Common
{
    public enum JoinType : int {
        Horizontal = 1,
        Vertical = 2
    }

    public enum JoinDirection : int {
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

    public class Join {
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

    public class StageInfo {
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

        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the absolute file path to the directory where this stage is stored
        /// </summary>
        public FilePath StagePath { get; set; }

        public FilePath MusicIntroPath { get; set; }
        public FilePath MusicLoopPath { get; set; }
        public int MusicNsfTrack { get; private set; }
        
        #endregion Properties

        public StageInfo() 
        {
            Screens = new Dictionary<string, ScreenInfo>();
            Joins = new List<Join>();
            continuePoints = new Dictionary<string, Point>();
        }

        public StageInfo(FilePath path) : this() 
        {
            LoadStageXml(path);
        }

        public void LoadStageXml(FilePath path) 
        {
            StagePath = path;

            var mapXml = XElement.Load(Path.Combine(StagePath.Absolute, "map.xml"));
            Name = Path.GetFileNameWithoutExtension(StagePath.Absolute);

            string tilePathRel = mapXml.Attribute("tiles").Value;
            tilePath = FilePath.FromRelative(tilePathRel, StagePath.Absolute);

            Tileset = new Tileset(tilePath.Absolute);

            PlayerStartX = 3;
            PlayerStartY = 3;

            LoadMusicXml(mapXml);
            LoadScreenXml(mapXml);

            XElement start = mapXml.Element("Start");
            if (start != null) 
            {
                int px, py;
                var screenAttr = start.Attribute("screen");
                if (screenAttr == null) throw new Exception("Start tag must have a screen attribute!");
                StartScreen = screenAttr.Value;
                if (!start.Attribute("x").Value.TryParse(out px)) throw new Exception("Start tag x is not a valid integer!");
                PlayerStartX = px;
                if (!start.Attribute("y").Value.TryParse(out py)) throw new Exception("Start tag y is not a valid integer!");
                PlayerStartY = py;
            }

            foreach (XElement contPoint in mapXml.Elements("Continue")) 
            {
                string screen = contPoint.Attribute("screen").Value;
                int x;
                contPoint.Attribute("x").Value.TryParse(out x);
                int y;
                contPoint.Attribute("y").Value.TryParse(out y);
                continuePoints.Add(screen, new Point(x, y));
            }

            foreach (XElement join in mapXml.Elements("Join")) 
            {
                string t = join.Attribute("type").Value;
                JoinType type;
                if (t.ToLower() == "horizontal") type = JoinType.Horizontal;
                else if (t.ToLower() == "vertical") type = JoinType.Vertical;
                else throw new Exception("map.xml file contains invalid join type.");

                string s1 = join.Attribute("s1").Value;
                string s2 = join.Attribute("s2").Value;
                int offset1;
                join.Attribute("offset1").Value.TryParse(out offset1);
                int offset2;
                join.Attribute("offset2").Value.TryParse(out offset2);
                int size;
                join.Attribute("size").Value.TryParse(out size);

                JoinDirection direction;
                XAttribute dirAttr = join.Attribute("direction");
                if (dirAttr == null || dirAttr.Value.ToUpper() == "BOTH") direction = JoinDirection.Both;
                else if (dirAttr.Value.ToUpper() == "FORWARD") direction = JoinDirection.ForwardOnly;
                else if (dirAttr.Value.ToUpper() == "BACKWARD") direction = JoinDirection.BackwardOnly;
                else throw new Exception("map.xml file contains invalid join direction.");

                string bosstile = null;
                XAttribute bossAttr = join.Attribute("bossdoor");
                bool bossdoor = (bossAttr != null);
                if (bossdoor) bosstile = bossAttr.Value;

                Join j = new Join();
                j.direction = direction;
                j.screenOne = s1;
                j.screenTwo = s2;
                j.offsetOne = offset1;
                j.offsetTwo = offset2;
                j.type = type;
                j.Size = size;
                j.bossDoor = bossdoor;
                j.bossEntityName = bosstile;

                Joins.Add(j);
            }
        }

        /* *
         * LoadMusicXml - Load xml data for music
         * */
        public void LoadMusicXml(XElement mapXml) 
        {
            var music = mapXml.Element("Music");
            if (music != null) 
            {
                var intro = music.Element("Intro");
                var loop = music.Element("Loop");
                MusicIntroPath = (intro != null) ? FilePath.FromRelative(intro.Value, StagePath.BasePath) : null;
                MusicLoopPath = (loop != null) ? FilePath.FromRelative(loop.Value, StagePath.BasePath) : null;

                XAttribute nsfAttr = music.Attribute("nsftrack");
                if (nsfAttr != null)
                {
                    int track;
                    if (!nsfAttr.Value.TryParse(out track)) throw new Exception("NSF track number is not a valid integer!");
                    MusicNsfTrack = track;
                }
            }
        }

        /* *
         * LoadScreenXml - Load xml data for screens
         * */
        public void LoadScreenXml(XElement mapXml) 
        {
            foreach (XElement screen in mapXml.Elements("Screen"))
            {
                ScreenInfo s = ScreenInfoFactory.FromXml(screen, StagePath, Tileset);
                this.Screens.Add(s.Name, s);
            }
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

        /// <summary>
        /// Changes the tileset by specifying an absolute path to the new tileset XML file.
        /// </summary>
        /// <param name="path">If it's not absolute, I'll make it so.</param>
        public void ChangeTileset(string path)
        {
            tilePath = FilePath.FromAbsolute(path, StagePath.Absolute);
            Tileset = new Tileset(tilePath.Absolute);
            
            foreach (ScreenInfo s in Screens.Values) s.Tileset = Tileset;
        }

        public void Clear() 
        {
            Screens.Clear();
            Joins.Clear();
            Tileset = null;
        }

        public void Save() { if (StagePath != null) Save(StagePath.Absolute); }

        /// <summary>
        /// Saves this stage to the specified directory.
        /// </summary>
        /// <param name="directory">An absolute path to the directory to save to.</param>
        public void Save(string directory)
        {
            StagePath = FilePath.FromAbsolute(directory, StagePath.BasePath);
            this.Name = Path.GetFileNameWithoutExtension(directory);

            XmlTextWriter writer = new XmlTextWriter(Path.Combine(StagePath.Absolute, "map.xml"), null);
            writer.Formatting = Formatting.Indented;
            writer.IndentChar = '\t';
            writer.Indentation = 1;

            writer.WriteStartElement("Map");
            writer.WriteAttributeString("name", Name);

            writer.WriteAttributeString("tiles", tilePath.Relative);

            if (this.MusicIntroPath != null || this.MusicLoopPath != null || this.MusicNsfTrack > 0)
            {
                writer.WriteStartElement("Music");
                if (MusicNsfTrack > 0) writer.WriteAttributeString("nsftrack", MusicNsfTrack.ToString());
                if (MusicIntroPath != null && !string.IsNullOrEmpty(MusicIntroPath.Relative)) writer.WriteElementString("Intro", MusicIntroPath.Relative);
                if (MusicLoopPath != null && !string.IsNullOrEmpty(MusicLoopPath.Relative)) writer.WriteElementString("Loop", MusicLoopPath.Relative);
                writer.WriteEndElement();
            }

            writer.WriteStartElement("Start");
            writer.WriteAttributeString("screen", StartScreen);
            writer.WriteAttributeString("x", PlayerStartX.ToString());
            writer.WriteAttributeString("y", PlayerStartY.ToString());
            writer.WriteEndElement();

            foreach (KeyValuePair<string, Point> pair in continuePoints)
            {
                writer.WriteStartElement("Continue");
                writer.WriteAttributeString("screen", pair.Key);
                writer.WriteAttributeString("x", pair.Value.X.ToString());
                writer.WriteAttributeString("y", pair.Value.Y.ToString());
                writer.WriteEndElement();
            }

            foreach (var screen in Screens.Values)
            {
                screen.Save(writer, StagePath);
            }

            foreach (Join join in Joins)
            {
                writer.WriteStartElement("Join");
                writer.WriteAttributeString("type", (join.type == JoinType.Horizontal) ? "horizontal" : "vertical");

                writer.WriteAttributeString("s1", join.screenOne);
                writer.WriteAttributeString("s2", join.screenTwo);
                writer.WriteAttributeString("offset1", join.offsetOne.ToString());
                writer.WriteAttributeString("offset2", join.offsetTwo.ToString());
                writer.WriteAttributeString("size", join.Size.ToString());
                switch (join.direction)
                {
                    case JoinDirection.Both: writer.WriteAttributeString("direction", "both"); break;
                    case JoinDirection.ForwardOnly: writer.WriteAttributeString("direction", "forward"); break;
                    case JoinDirection.BackwardOnly: writer.WriteAttributeString("direction", "backward"); break;
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Close();
        }

        // this doesn't work for files on different drives
        // also right now relativeTo should not have a trailing slash.
        internal static string PathToRelative(string path, string relativeTo)
        {
            if (System.IO.Path.HasExtension(relativeTo))
            {
                relativeTo = System.IO.Path.GetDirectoryName(relativeTo);
            }
            path = System.IO.Path.GetFullPath(path);

            // split into directories
            string[] pathdirs = path.Split(System.IO.Path.DirectorySeparatorChar);
            string[] reldirs = relativeTo.Split(System.IO.Path.DirectorySeparatorChar);

            int length = Math.Min(pathdirs.Length, reldirs.Length);
            StringBuilder relativePath = new StringBuilder();

            // find where the paths differ
            int forkpoint = 0;
            while (forkpoint < length && pathdirs[forkpoint] == reldirs[forkpoint]) forkpoint++;

            // go back by the number of directories in the relativeTo path
            int dirs = reldirs.Length - forkpoint;
            for (int i = 0; i < dirs; i++) relativePath.Append("..").Append(System.IO.Path.DirectorySeparatorChar);

            // append file path from that directory
            for (int i = forkpoint; i < pathdirs.Length - 1; i++) relativePath.Append(pathdirs[i]).Append(System.IO.Path.DirectorySeparatorChar);
            // append file, without directory separator
            relativePath.Append(pathdirs[pathdirs.Length - 1]);

            return relativePath.ToString();
        }
    }
}
