using System.Xml;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO.Xml
{
    internal class SoundXmlWriter
    {
        public void Write(SoundInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Sound");
            writer.WriteAttributeString("name", info.Name);
            writer.WriteAttributeString("loop", info.Loop.ToString());
            writer.WriteAttributeString("volume", info.Volume.ToString());

            if (info.Type == AudioType.Wav && info.Path != null)
                writer.WriteAttributeString("path", info.Path.Relative);

            if (info.Type == AudioType.Nsf)
            {
                writer.WriteAttributeString("track", info.NsfTrack.ToString());
                writer.WriteAttributeString("priority", info.Priority.ToString());
            }

            writer.WriteEndElement();
        }
    }
}
