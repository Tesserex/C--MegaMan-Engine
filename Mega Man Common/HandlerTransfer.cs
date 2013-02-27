using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public enum HandlerType
    {
        Stage,
        Scene,
        StageSelect,
        Menu
    }

    public enum HandlerMode
    {
        Next,
        Push,
        Pop
    }

    public class HandlerTransfer
    {
        public HandlerType Type;
        public HandlerMode Mode;
        public string Name;
        public bool Fade;
        public bool Pause;

        public static HandlerTransfer FromXml(XElement node)
        {
            HandlerTransfer transfer = new HandlerTransfer();

            var modeAttr = node.Attribute("mode");
            var mode = HandlerMode.Next;
            if (modeAttr != null)
            {
                Enum.TryParse<HandlerMode>(modeAttr.Value, true, out mode);
            }

            transfer.Mode = mode;

            if (mode == HandlerMode.Push)
            {
                transfer.Pause = node.TryAttribute<bool>("pause");
            }

            if (mode != HandlerMode.Pop)
            {
                switch (node.RequireAttribute("type").Value.ToLower())
                {
                    case "stage":
                        transfer.Type = HandlerType.Stage;
                        break;

                    case "stageselect":
                        transfer.Type = HandlerType.StageSelect;
                        break;

                    case "scene":
                        transfer.Type = HandlerType.Scene;
                        break;

                    case "menu":
                        transfer.Type = HandlerType.Menu;
                        break;
                }

                transfer.Name = node.RequireAttribute("name").Value;
            }

            transfer.Fade = node.TryAttribute<bool>("fade");

            return transfer;
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Next");

            if (Mode != HandlerMode.Next)
            {
                writer.WriteAttributeString("mode", Mode.ToString());
            }

            if (Mode == HandlerMode.Push)
            {
                writer.WriteAttributeString("pause", Pause.ToString());
            }

            if (Mode != HandlerMode.Pop)
            {
                writer.WriteAttributeString("type", Enum.GetName(typeof(HandlerType), Type));
                writer.WriteAttributeString("name", Name);
            }

            writer.WriteAttributeString("fade", Fade.ToString());

            writer.WriteEndElement();
        }
    }
}
