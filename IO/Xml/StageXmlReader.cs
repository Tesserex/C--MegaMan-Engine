using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.IO.DataSources;
using MegaMan.IO.Xml.Handlers.Commands;

namespace MegaMan.IO.Xml
{
    internal class StageXmlReader : IStageReader
    {
        private StageInfo _info;

        private readonly IReaderProvider _readerProvider;
        private readonly EntityPlacementXmlReader _entityReader;
        private readonly HandlerCommandXmlReader _commandReader;
        private BlockPatternXmlReader _blockReader = new BlockPatternXmlReader();
        private IDataSource _dataSource;

        public void Init(IDataSource dataSource)
        {
            this._dataSource = dataSource;
        }

        public StageXmlReader(IReaderProvider readerProvider, EntityPlacementXmlReader entityReader, HandlerCommandXmlReader commandReader)
        {
            _readerProvider = readerProvider;
            _entityReader = entityReader;
            _commandReader = commandReader;
        }

        public StageInfo Load(FilePath path)
        {
            _info = new StageInfo();
            
            _info.StoragePath = path;

            var mapPath = Path.Combine(_info.StoragePath.Absolute, "map.xml");
            var stream = _dataSource.GetData(FilePath.FromAbsolute(mapPath, _info.StoragePath.BasePath));
            var mapXml = XElement.Load(stream);
            _info.Name = mapXml.TryAttribute("name", Path.GetFileNameWithoutExtension(_info.StoragePath.Absolute));

            string tilePathRel = mapXml.Attribute("tiles").Value;
            var tilePath = FilePath.FromRelative(tilePathRel, _info.StoragePath.BasePath);

            var tileReader = _readerProvider.GetTilesetReader(tilePath);
            var tileset = tileReader.Load(tilePath);
            _info.ChangeTileset(tileset);

            _info.PlayerStartX = 3;
            _info.PlayerStartY = 3;

            LoadMusicXml(mapXml);
            LoadScreens(mapXml);

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

            stream.Close();

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
                _info.MusicIntroPath = (intro != null) ? FilePath.FromRelative(intro.Value, _info.StoragePath.BasePath) : null;
                _info.MusicLoopPath = (loop != null) ? FilePath.FromRelative(loop.Value, _info.StoragePath.BasePath) : null;
                _info.MusicNsfTrack = music.TryAttribute<int>("nsftrack");
            }
        }

        /* *
         * LoadScreenXml - Load xml data for screens
         * */
        public void LoadScreens(XElement mapXml)
        {
            foreach (XElement screen in mapXml.Elements("Screen"))
            {
                ScreenInfo s = LoadScreenXml(screen, _info.StoragePath, _info.Tileset);
                _info.Screens.Add(s.Name, s);
            }
        }

        private ScreenInfo LoadScreenXml(XElement node, FilePath stagePath, Tileset tileset)
        {
            string id = node.RequireAttribute("id").Value;

            var screen = new ScreenInfo(id, tileset);

            screen.Layers.Add(LoadScreenLayer(node, stagePath, id, tileset, 0, 0, false));

            foreach (var overlay in node.Elements("Overlay"))
            {
                var name = overlay.RequireAttribute("name").Value;
                var x = overlay.TryAttribute<int>("x");
                var y = overlay.TryAttribute<int>("y");
                bool foreground = overlay.TryAttribute<bool>("foreground");
                bool parallax = overlay.TryAttribute<bool>("parallax");

                var layer = LoadScreenLayer(overlay, stagePath, name, tileset, x, y, foreground);
                layer.Parallax = parallax;

                screen.Layers.Add(layer);
            }

            foreach (XElement teleport in node.Elements("Teleport"))
            {
                TeleportInfo info;
                int from_x = teleport.TryAttribute<int>("from_x");
                int from_y = teleport.TryAttribute<int>("from_y");
                int to_x = teleport.TryAttribute<int>("to_x");
                int to_y = teleport.TryAttribute<int>("to_y");
                info.From = new Point(from_x, from_y);
                info.To = new Point(to_x, to_y);
                info.TargetScreen = teleport.Attribute("to_screen").Value;

                screen.Teleports.Add(info);
            }

            var blocks = new List<BlockPatternInfo>();

            foreach (XElement blockNode in node.Elements("Blocks"))
            {
                BlockPatternInfo pattern = _blockReader.FromXml(blockNode);
                screen.BlockPatterns.Add(pattern);
            }

            screen.Commands = _commandReader.LoadCommands(node, stagePath.BasePath);

            return screen;
        }

        private ScreenLayerInfo LoadScreenLayer(XElement node, FilePath stagePath, string name, Tileset tileset, int tileStartX, int tileStartY, bool foreground)
        {
            var tileFilePath = Path.Combine(stagePath.Absolute, name + ".scn");

            var tileArray = LoadTiles(FilePath.FromAbsolute(tileFilePath, stagePath.BasePath));
            var tileLayer = new TileLayer(tileArray, tileset, tileStartX, tileStartY);

            var keyframes = new List<ScreenLayerKeyframe>();
            foreach (var keyframeNode in node.Elements("Keyframe"))
            {
                var frame = LoadScreenLayerKeyFrame(keyframeNode);
                keyframes.Add(frame);
            }

            var layer = new ScreenLayerInfo(name, tileLayer, foreground, keyframes);

            foreach (XElement entity in node.Elements("Entity"))
            {
                EntityPlacement info = _entityReader.Load(entity);
                layer.AddEntity(info);
            }

            return layer;
        }

        private static ScreenLayerKeyframe LoadScreenLayerKeyFrame(XElement node)
        {
            var frameNumber = node.GetAttribute<int>("frame");

            var keyframe = new ScreenLayerKeyframe();
            keyframe.Frame = frameNumber;

            var moveNode = node.Element("Move");
            if (moveNode != null)
            {
                var moveInfo = new ScreenLayerMoveCommand();
                moveInfo.X = moveNode.GetAttribute<int>("x");
                moveInfo.Y = moveNode.GetAttribute<int>("y");
                moveInfo.Duration = moveNode.GetAttribute<int>("duration");

                keyframe.Move = moveInfo;
            }

            keyframe.Reset = node.Elements("Reset").Any();

            return keyframe;
        }

        private int[,] LoadTiles(FilePath filepath)
        {
            List<string> lines = new List<string>();
            using (var stream = _dataSource.GetData(filepath))
            {
                using (var text = new StreamReader(stream))
                {
                    while (!text.EndOfStream)
                    {
                        lines.Add(text.ReadLine());
                    }
                }
            }

            string[] firstline = lines[0].Split(' ');
            int width = int.Parse(firstline[0]);
            int height = int.Parse(firstline[1]);

            int[,] tiles = new int[width, height];
            for (int y = 0; y < height; y++)
            {
                string[] line = lines[y + 1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < width; x++)
                {
                    int id = int.Parse(line[x]);
                    tiles[x, y] = id;
                }
            }

            return tiles;
        }
    }
}
