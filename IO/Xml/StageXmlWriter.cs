using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.IO.Xml.Handlers;
using MegaMan.IO.Xml.Handlers.Commands;

namespace MegaMan.IO.Xml
{
    public class StageXmlWriter : IStageWriter
    {
        private StageInfo _stageInfo;
        private XmlTextWriter _writer;
        private readonly HandlerCommandXmlWriter _commandWriter;
        private readonly EntityPlacementXmlWriter _entityWriter;

        internal StageXmlWriter(HandlerCommandXmlWriter commandWriter, EntityPlacementXmlWriter entityWriter)
        {
            _commandWriter = commandWriter;
            _entityWriter = entityWriter;
        }

        public void Save(StageInfo stage)
        {
            this._stageInfo = stage;
            _writer = new XmlTextWriter(Path.Combine(_stageInfo.StagePath.Absolute, "map.xml"), Encoding.Default);
            _writer.Formatting = Formatting.Indented;
            _writer.Indentation = 1;
            _writer.IndentChar = '\t';

            _writer.WriteStartElement("Map");
            _writer.WriteAttributeString("name", _stageInfo.Name);

            _writer.WriteAttributeString("tiles", _stageInfo.Tileset.FilePath.Relative);

            if (_stageInfo.MusicIntroPath != null || _stageInfo.MusicLoopPath != null || _stageInfo.MusicNsfTrack > 0)
            {
                _writer.WriteStartElement("Music");
                if (_stageInfo.MusicNsfTrack > 0) _writer.WriteAttributeString("nsftrack", _stageInfo.MusicNsfTrack.ToString());
                if (_stageInfo.MusicIntroPath != null && !string.IsNullOrEmpty(_stageInfo.MusicIntroPath.Relative)) _writer.WriteElementString("Intro", _stageInfo.MusicIntroPath.Relative);
                if (_stageInfo.MusicLoopPath != null && !string.IsNullOrEmpty(_stageInfo.MusicLoopPath.Relative)) _writer.WriteElementString("Loop", _stageInfo.MusicLoopPath.Relative);
                _writer.WriteEndElement();
            }

            _writer.WriteStartElement("Start");
            _writer.WriteAttributeString("screen", _stageInfo.StartScreen);
            _writer.WriteAttributeString("x", _stageInfo.PlayerStartX.ToString());
            _writer.WriteAttributeString("y", _stageInfo.PlayerStartY.ToString());
            _writer.WriteEndElement();

            foreach (KeyValuePair<string, Point> pair in _stageInfo.ContinuePoints)
            {
                _writer.WriteStartElement("Continue");
                _writer.WriteAttributeString("screen", pair.Key);
                _writer.WriteAttributeString("x", pair.Value.X.ToString());
                _writer.WriteAttributeString("y", pair.Value.Y.ToString());
                _writer.WriteEndElement();
            }

            foreach (var screen in _stageInfo.Screens.Values)
            {
                SaveScreen(screen);
            }

            foreach (Join join in _stageInfo.Joins)
            {
                _writer.WriteStartElement("Join");
                _writer.WriteAttributeString("type", (join.type == JoinType.Horizontal) ? "horizontal" : "vertical");

                _writer.WriteAttributeString("s1", join.screenOne);
                _writer.WriteAttributeString("s2", join.screenTwo);
                _writer.WriteAttributeString("offset1", join.offsetOne.ToString());
                _writer.WriteAttributeString("offset2", join.offsetTwo.ToString());
                _writer.WriteAttributeString("size", join.Size.ToString());
                switch (join.direction)
                {
                    case JoinDirection.Both: _writer.WriteAttributeString("direction", "both"); break;
                    case JoinDirection.ForwardOnly: _writer.WriteAttributeString("direction", "forward"); break;
                    case JoinDirection.BackwardOnly: _writer.WriteAttributeString("direction", "backward"); break;
                }

                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
            _writer.Close();
        }

        public void SaveScreen(ScreenInfo screen)
        {
            _writer.WriteStartElement("Screen");
            _writer.WriteAttributeString("id", screen.Name);

            foreach (var command in screen.Commands)
            {
                if (!(command is SceneEntityCommandInfo))
                    _commandWriter.Write(command, _writer);
            }

            foreach (var entity in screen.Layers[0].Entities)
                _entityWriter.Write(entity, _writer);

            foreach (var layer in screen.Layers.Skip(1))
            {
                layer.Save(_writer);
            }

            foreach (BlockPatternInfo pattern in screen.BlockPatterns)
            {
                //pattern.Save(writer);
            }

            foreach (TeleportInfo teleport in screen.Teleports)
            {
                _writer.WriteStartElement("Teleport");
                _writer.WriteAttributeString("from_x", teleport.From.X.ToString());
                _writer.WriteAttributeString("from_y", teleport.From.Y.ToString());
                _writer.WriteAttributeString("to_screen", teleport.TargetScreen);
                _writer.WriteAttributeString("to_x", teleport.To.X.ToString());
                _writer.WriteAttributeString("to_y", teleport.To.Y.ToString());
                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();

            foreach (var layer in screen.Layers)
            {
                layer.Tiles.Save(Path.Combine(_stageInfo.StagePath.Absolute, layer.Name + ".scn"));
            }
        }
    }
}
