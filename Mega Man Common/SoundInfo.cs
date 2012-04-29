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

        public static SoundInfo FromXml(XElement soundNode, string basePath)
        {
            SoundInfo sound = new SoundInfo {Name = soundNode.RequireAttribute("name").Value};

            bool loop;
            soundNode.TryBool("loop", out loop);
            sound.Loop = loop;

            float vol;
            if (!soundNode.TryFloat("volume", out vol)) vol = 1;
            sound.Volume = vol;

            XAttribute pathattr = soundNode.Attribute("path");
            XAttribute trackAttr = soundNode.Attribute("track");
            if (pathattr != null)
            {
                sound.Type = AudioType.Wav;
                sound.Path = FilePath.FromRelative(pathattr.Value, basePath);
            }
            else if (trackAttr != null)
            {
                sound.Type = AudioType.NSF;

                int track;
                if (!trackAttr.Value.TryParse(out track) || track <= 0) throw new GameXmlException(trackAttr, "Sound track attribute must be an integer greater than zero.");
                sound.NsfTrack = track;

                int priority;
                if (!soundNode.TryInteger("priority", out priority)) priority = 100;
                sound.Priority = (byte)priority;
            }
            else
            {
                sound.Type = AudioType.Unknown;
            }

            return sound;
        }

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
