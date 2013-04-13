using MegaMan.Common;
using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class StageXmlReader
    {
        private StageInfo _info;

        public StageInfo LoadStageXml(FilePath path)
        {
            _info = new StageInfo();

            _info.StagePath = path;

            var mapXml = XElement.Load(Path.Combine(_info.StagePath.Absolute, "map.xml"));
            _info.Name = Path.GetFileNameWithoutExtension(_info.StagePath.Absolute);

            string tilePathRel = mapXml.Attribute("tiles").Value;
            var tilePath = FilePath.FromRelative(tilePathRel, _info.StagePath.Absolute);

            _info.ChangeTileset(tilePath.Absolute);

            _info.PlayerStartX = 3;
            _info.PlayerStartY = 3;

            LoadMusicXml(mapXml);
            LoadScreenXml(mapXml);

            XElement start = mapXml.Element("Start");
            if (start != null)
            {
                _info.StartScreen = start.RequireAttribute("screen").Value;
                _info.PlayerStartX = start.GetAttribute<int>("x");
                _info.PlayerStartY = start.GetAttribute<int>("y");
            }

            foreach (XElement contPoint in mapXml.Elements("Continue"))
            {
                string screen = contPoint.GetAttribute<string>("screen");
                int x = contPoint.GetAttribute<int>("x");
                int y = contPoint.GetAttribute<int>("y");
                _info.AddContinuePoint(screen, new Point(x, y));
            }

            foreach (XElement join in mapXml.Elements("Join"))
            {
                string t = join.Attribute("type").Value;
                JoinType type;
                if (t.ToLower() == "horizontal") type = JoinType.Horizontal;
                else if (t.ToLower() == "vertical") type = JoinType.Vertical;
                else throw new GameXmlException(join, "map.xml file contains invalid join type.");

                string s1 = join.RequireAttribute("s1").Value;
                string s2 = join.RequireAttribute("s2").Value;
                int offset1 = join.GetAttribute<int>("offset1");
                int offset2 = join.GetAttribute<int>("offset2");
                int size = join.GetAttribute<int>("size");

                JoinDirection direction;
                XAttribute dirAttr = join.Attribute("direction");
                if (dirAttr == null || dirAttr.Value.ToUpper() == "BOTH") direction = JoinDirection.Both;
                else if (dirAttr.Value.ToUpper() == "FORWARD") direction = JoinDirection.ForwardOnly;
                else if (dirAttr.Value.ToUpper() == "BACKWARD") direction = JoinDirection.BackwardOnly;
                else throw new GameXmlException(dirAttr, "map.xml file contains invalid join direction.");

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

                _info.Joins.Add(j);
            }

            return _info;
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
                _info.MusicIntroPath = (intro != null) ? FilePath.FromRelative(intro.Value, _info.StagePath.BasePath) : null;
                _info.MusicLoopPath = (loop != null) ? FilePath.FromRelative(loop.Value, _info.StagePath.BasePath) : null;
                _info.MusicNsfTrack = music.TryAttribute<int>("nsftrack");
            }
        }

        /* *
         * LoadScreenXml - Load xml data for screens
         * */
        public void LoadScreenXml(XElement mapXml)
        {
            foreach (XElement screen in mapXml.Elements("Screen"))
            {
                ScreenInfo s = ScreenInfoFactory.FromXml(screen, _info.StagePath, _info.Tileset);
                _info.Screens.Add(s.Name, s);
            }
        }
    }
}
