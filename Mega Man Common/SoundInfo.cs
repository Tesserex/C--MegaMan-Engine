using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public class SoundInfo
    {
        public string Name { get; set; }
        public FilePath Path { get; set; }
        public int NsfTrack { get; set; }
        public bool Loop { get; set; }
        public float Volume { get; set; }
        public byte Priority { get; set; }
        public AudioType Type { get; set; }

        public void Save(XmlTextWriter writer)
        {
            if (Type == AudioType.Unknown) return;

            writer.WriteStartElement("Sound");
            writer.WriteAttributeString("name", Name);

            if (Type == AudioType.Wav)
            {
                writer.WriteAttributeString("path", Path.Relative);
            }
            else
            {
                writer.WriteAttributeString("track", NsfTrack.ToString());
            }

            writer.WriteAttributeString("priority", Priority.ToString());
            if (Loop) writer.WriteAttributeString("loop", Loop.ToString());
            if (Volume < 1) writer.WriteAttributeString("volume", Volume.ToString());
            writer.WriteEndElement();
        }
    }
}
