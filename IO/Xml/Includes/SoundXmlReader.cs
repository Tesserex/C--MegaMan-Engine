using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO.Xml.Includes
{
    internal class SoundXmlReader : IIncludeXmlReader
    {
        public void Load(Project project, XElement xmlNode)
        {
            project.AddSound(LoadSound(xmlNode, project.BaseDir));
        }

        private SoundInfo LoadSound(XElement soundNode, string basePath)
        {
            SoundInfo sound = new SoundInfo { Name = soundNode.RequireAttribute("name").Value };

            sound.Loop = soundNode.TryAttribute<bool>("loop");

            sound.Volume = soundNode.TryAttribute<float>("volume", 1);

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

                sound.Priority = soundNode.TryAttribute<byte>("priority", 100);
            }
            else
            {
                sound.Type = AudioType.Unknown;
            }

            return sound;
        }

        public string NodeName
        {
            get { return "Sound"; }
        }
    }
}
