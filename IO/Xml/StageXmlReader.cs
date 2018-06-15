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
        private StageInfo info;

        private readonly IReaderProvider readerProvider;
        private readonly EntityPlacementXmlReader entityReader;
        private readonly HandlerCommandXmlReader commandReader;
        private BlockPatternXmlReader blockReader = new BlockPatternXmlReader();
        private IDataSource dataSource;

        public void Init(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        public StageXmlReader(IReaderProvider readerProvider, EntityPlacementXmlReader entityReader, HandlerCommandXmlReader commandReader)
        {
            this.readerProvider = readerProvider;
            this.entityReader = entityReader;
            this.commandReader = commandReader;
        }

        public StageInfo Load(FilePath path)
        {
            info = new StageInfo();
            
            info.StoragePath = path;

            var mapPath = Path.Combine(info.StoragePath.Absolute, "map.xml");
            var stream = dataSource.GetData(FilePath.FromAbsolute(mapPath, info.StoragePath.BasePath));
            var mapXml = XElement.Load(stream);
            info.Name = mapXml.TryAttribute("name", Path.GetFileNameWithoutExtension(info.StoragePath.Absolute));

            var tilePathRel = mapXml.Attribute("tiles").Value;
            var tilePath = FilePath.FromRelative(tilePathRel, info.StoragePath.BasePath);

            var tileReader = readerProvider.GetTilesetReader(tilePath);
            var tileset = tileReader.Load(tilePath);
            info.ChangeTileset(tileset);

            info.PlayerStartX = 3;
            info.PlayerStartY = 3;

            LoadMusicXml(mapXml);
            LoadScreens(mapXml);

            var start = mapXml.Element("Start");
            if (start != null)
            {
                info.StartScreen = start.RequireAttribute("screen").Value;
                info.PlayerStartX = start.GetAttribute<int>("x");
                info.PlayerStartY = start.GetAttribute<int>("y");
            }

            foreach (var contPoint in mapXml.Elements("Continue"))
            {
                var screen = contPoint.GetAttribute<string>("screen");
                var x = contPoint.GetAttribute<int>("x");
                var y = contPoint.GetAttribute<int>("y");
                info.AddContinuePoint(screen, new Point(x, y));
            }

            foreach (var join in mapXml.Elements("Join"))
            {
                var t = join.Attribute("type").Value;
                JoinType type;
                if (t.ToLower() == "horizontal") type = JoinType.Horizontal;
                else if (t.ToLower() == "vertical") type = JoinType.Vertical;
                else throw new GameXmlException(join, "map.xml file contains invalid join type.");

                var s1 = join.RequireAttribute("s1").Value;
                var s2 = join.RequireAttribute("s2").Value;
                var offset1 = join.GetAttribute<int>("offset1");
                var offset2 = join.GetAttribute<int>("offset2");
                var size = join.GetAttribute<int>("size");

                JoinDirection direction;
                var dirAttr = join.Attribute("direction");
                if (dirAttr == null || dirAttr.Value.ToUpper() == "BOTH") direction = JoinDirection.Both;
                else if (dirAttr.Value.ToUpper() == "FORWARD") direction = JoinDirection.ForwardOnly;
                else if (dirAttr.Value.ToUpper() == "BACKWARD") direction = JoinDirection.BackwardOnly;
                else throw new GameXmlException(dirAttr, "map.xml file contains invalid join direction.");

                string bosstile = null;
                var bossAttr = join.Attribute("bossdoor");
                var bossdoor = (bossAttr != null);
                if (bossdoor) bosstile = bossAttr.Value;

                var j = new Join();
                j.direction = direction;
                j.screenOne = s1;
                j.screenTwo = s2;
                j.offsetOne = offset1;
                j.offsetTwo = offset2;
                j.type = type;
                j.Size = size;
                j.bossDoor = bossdoor;
                j.bossEntityName = bosstile;

                info.Joins.Add(j);
            }

            stream.Close();

            return info;
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
                info.MusicIntroPath = (intro != null) ? FilePath.FromRelative(intro.Value, info.StoragePath.BasePath) : null;
                info.MusicLoopPath = (loop != null) ? FilePath.FromRelative(loop.Value, info.StoragePath.BasePath) : null;
                info.MusicNsfTrack = music.TryAttribute<int>("nsftrack");
            }
        }

        /* *
         * LoadScreenXml - Load xml data for screens
         * */
        public void LoadScreens(XElement mapXml)
        {
            foreach (var screen in mapXml.Elements("Screen"))
            {
                var s = LoadScreenXml(screen, info.StoragePath, info.Tileset);
                info.Screens.Add(s.Name, s);
            }
        }

        private ScreenInfo LoadScreenXml(XElement node, FilePath stagePath, Tileset tileset)
        {
            var id = node.RequireAttribute("id").Value;

            var screen = new ScreenInfo(id, tileset);

            screen.Layers.Add(LoadScreenLayer(node, stagePath, id, tileset, 0, 0, false));

            foreach (var overlay in node.Elements("Overlay"))
            {
                var name = overlay.RequireAttribute("name").Value;
                var x = overlay.TryAttribute<int>("x");
                var y = overlay.TryAttribute<int>("y");
                var foreground = overlay.TryAttribute<bool>("foreground");
                var parallax = overlay.TryAttribute<bool>("parallax");

                var layer = LoadScreenLayer(overlay, stagePath, name, tileset, x, y, foreground);
                layer.Parallax = parallax;

                screen.Layers.Add(layer);
            }

            foreach (var teleport in node.Elements("Teleport"))
            {
                TeleportInfo info;
                var fromX = teleport.TryAttribute<int>("from_x");
                var fromY = teleport.TryAttribute<int>("from_y");
                var toX = teleport.TryAttribute<int>("to_x");
                var toY = teleport.TryAttribute<int>("to_y");
                info.From = new Point(fromX, fromY);
                info.To = new Point(toX, toY);
                info.TargetScreen = teleport.Attribute("to_screen").Value;

                screen.Teleports.Add(info);
            }

            var blocks = new List<BlockPatternInfo>();

            foreach (var blockNode in node.Elements("Blocks"))
            {
                var pattern = blockReader.FromXml(blockNode);
                screen.BlockPatterns.Add(pattern);
            }

            screen.Commands = commandReader.LoadCommands(node, stagePath.BasePath);

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

            foreach (var entity in node.Elements("Entity"))
            {
                var info = entityReader.Load(entity);
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
            var lines = new List<string>();
            using (var stream = dataSource.GetData(filepath))
            {
                using (var text = new StreamReader(stream))
                {
                    while (!text.EndOfStream)
                    {
                        lines.Add(text.ReadLine());
                    }
                }
            }

            var firstline = lines[0].Split(' ');
            var width = int.Parse(firstline[0]);
            var height = int.Parse(firstline[1]);

            var tiles = new int[width, height];
            for (var y = 0; y < height; y++)
            {
                var line = lines[y + 1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (var x = 0; x < width; x++)
                {
                    var id = int.Parse(line[x]);
                    tiles[x, y] = id;
                }
            }

            return tiles;
        }
    }
}
