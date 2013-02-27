using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public class SceneInfo : HandlerInfo
    {
        public int Duration { get; set; }
        public bool CanSkip { get; set; }

        public List<KeyFrameInfo> KeyFrames { get; private set; }
        public HandlerTransfer NextHandler { get; private set; }

        public SceneInfo()
        {
            KeyFrames = new List<KeyFrameInfo>();
        }

        public static SceneInfo FromXml(XElement node, string basePath)
        {
            var info = new SceneInfo();

            info.Load(node, basePath);

            return info;
        }

        protected override void Load(XElement node, string basePath)
        {
            base.Load(node, basePath);

            this.Duration = node.GetAttribute<int>("duration");

            this.CanSkip = node.TryAttribute<bool>("canskip");

            foreach (var keyNode in node.Elements("Keyframe"))
            {
                this.KeyFrames.Add(KeyFrameInfo.FromXml(keyNode, basePath));
            }

            var transferNode = node.Element("Next");
            if (transferNode != null)
            {
                this.NextHandler = HandlerTransfer.FromXml(transferNode);
            }
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Scene");

            base.Save(writer);

            writer.WriteAttributeString("duration", Duration.ToString());
            writer.WriteAttributeString("canskip", CanSkip.ToString());

            foreach (var keyframe in KeyFrames)
            {
                keyframe.Save(writer);
            }

            if (NextHandler != null)
            {
                NextHandler.Save(writer);
            }

            writer.WriteEndElement();
        }
    }

    public class KeyFrameInfo
    {
        public int Frame { get; set; }
        public bool Fade { get; set; }
        public List<SceneCommandInfo> Commands { get; private set; }

        public static KeyFrameInfo FromXml(XElement node, string basePath)
        {
            var info = new KeyFrameInfo();

            info.Frame = node.GetAttribute<int>("frame");

            info.Fade = node.TryAttribute<bool>("fade");

            info.Commands = SceneCommandInfo.Load(node, basePath);

            return info;
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Keyframe");

            foreach (var command in Commands)
            {
                command.Save(writer);
            }

            writer.WriteEndElement();
        }
    }
}
