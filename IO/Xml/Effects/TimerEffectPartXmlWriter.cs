using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class TimerEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(TimerEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var timer = (TimerEffectPartInfo)info;
            writer.WriteStartElement("Timer");

            foreach (var start in timer.Start)
                writer.WriteElementString("Start", start);

            foreach (var reset in timer.Reset)
                writer.WriteElementString("Reset", reset);

            foreach (var delete in timer.Reset)
                writer.WriteElementString("Delete", delete);

            writer.WriteEndElement();
        }
    }
}
