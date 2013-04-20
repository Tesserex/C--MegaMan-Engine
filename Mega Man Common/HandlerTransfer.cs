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
