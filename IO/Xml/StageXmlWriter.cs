using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using MegaMan.Common;
using MegaMan.IO.Xml.Handlers.Commands;

namespace MegaMan.IO.Xml
{
    public class StageXmlWriter : IStageWriter
    {
        private StageInfo stageInfo;
        private XmlTextWriter writer;
        private readonly HandlerCommandXmlWriter commandWriter;
        private readonly EntityPlacementXmlWriter entityWriter;

        public StageXmlWriter(HandlerCommandXmlWriter commandWriter, EntityPlacementXmlWriter entityWriter)
        {
            this.commandWriter = commandWriter;
            this.entityWriter = entityWriter;
        }

        public void Save(StageInfo stage)
        {
            stageInfo = stage;
            writer = new XmlTextWriter(Path.Combine(stageInfo.StoragePath.Absolute, "map.xml"), Encoding.Default);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartElement("Map");
            writer.WriteAttributeString("name", stageInfo.Name);

            writer.WriteAttributeString("tiles", stageInfo.Tileset.FilePath.Relative);

            if (stageInfo.MusicIntroPath != null || stageInfo.MusicLoopPath != null || stageInfo.MusicNsfTrack > 0)
            {
                writer.WriteStartElement("Music");
                if (stageInfo.MusicNsfTrack > 0) writer.WriteAttributeString("nsftrack", stageInfo.MusicNsfTrack.ToString());
                if (stageInfo.MusicIntroPath != null && !string.IsNullOrEmpty(stageInfo.MusicIntroPath.Relative)) writer.WriteElementString("Intro", stageInfo.MusicIntroPath.Relative);
                if (stageInfo.MusicLoopPath != null && !string.IsNullOrEmpty(stageInfo.MusicLoopPath.Relative)) writer.WriteElementString("Loop", stageInfo.MusicLoopPath.Relative);
                writer.WriteEndElement();
            }

            writer.WriteStartElement("Start");
            writer.WriteAttributeString("screen", stageInfo.StartScreen);
            writer.WriteAttributeString("x", stageInfo.PlayerStartX.ToString());
            writer.WriteAttributeString("y", stageInfo.PlayerStartY.ToString());
            writer.WriteEndElement();

            foreach (var pair in stageInfo.ContinuePoints)
            {
                writer.WriteStartElement("Continue");
                writer.WriteAttributeString("screen", pair.Key);
                writer.WriteAttributeString("x", pair.Value.X.ToString());
                writer.WriteAttributeString("y", pair.Value.Y.ToString());
                writer.WriteEndElement();
            }

            foreach (var screen in stageInfo.Screens.Values)
            {
                SaveScreen(screen);
            }

            foreach (var join in stageInfo.Joins)
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

        public void SaveScreen(ScreenInfo screen)
        {
            writer.WriteStartElement("Screen");
            writer.WriteAttributeString("id", screen.Name);

            foreach (var command in screen.Commands)
            {
                if (!(command is SceneEntityCommandInfo))
                    commandWriter.Write(command, writer);
            }

            foreach (var entity in screen.Layers[0].Entities)
                entityWriter.Write(entity, writer);

            foreach (var layer in screen.Layers.Skip(1))
            {
                SaveLayer(layer);
            }

            foreach (var pattern in screen.BlockPatterns)
            {
                //pattern.Save(writer);
            }

            foreach (var teleport in screen.Teleports)
            {
                writer.WriteStartElement("Teleport");
                writer.WriteAttributeString("from_x", teleport.From.X.ToString());
                writer.WriteAttributeString("from_y", teleport.From.Y.ToString());
                writer.WriteAttributeString("to_screen", teleport.TargetScreen);
                writer.WriteAttributeString("to_x", teleport.To.X.ToString());
                writer.WriteAttributeString("to_y", teleport.To.Y.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            foreach (var layer in screen.Layers)
            {
                layer.Tiles.Save(Path.Combine(stageInfo.StoragePath.Absolute, layer.Name + ".scn"));
            }
        }

        private void SaveLayer(ScreenLayerInfo layer)
        {
            writer.WriteStartElement("Overlay");

            writer.WriteAttributeString("name", layer.Name);

            writer.WriteAttributeString("x", layer.Tiles.BaseX.ToString());
            writer.WriteAttributeString("y", layer.Tiles.BaseY.ToString());

            if (layer.Foreground)
                writer.WriteAttributeString("foreground", layer.Foreground.ToString());

            if (layer.Parallax)
                writer.WriteAttributeString("parallax", layer.Parallax.ToString());

            foreach (var entity in layer.Entities)
                entityWriter.Write(entity, writer);

            foreach (var keyframe in layer.Keyframes)
                SaveKeyframe(keyframe);

            writer.WriteEndElement();
        }

        private void SaveKeyframe(ScreenLayerKeyframe frame)
        {
            writer.WriteStartElement("Keyframe");

            writer.WriteAttributeString("frame", frame.Frame.ToString());

            if (frame.Move != null)
            {
                writer.WriteStartElement("Move");
                writer.WriteAttributeString("x", frame.Move.X.ToString());
                writer.WriteAttributeString("y", frame.Move.Y.ToString());
                writer.WriteAttributeString("duration", frame.Move.Duration.ToString());
                writer.WriteEndElement();
            }

            if (frame.Reset)
                writer.WriteElementString("Reset", "");

            writer.WriteEndElement();
        }
    }
}
