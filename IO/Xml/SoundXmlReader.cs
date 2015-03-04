using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class SoundXmlReader : IGameObjectXmlReader
    {
        public void Load(Project project, XElement node)
        {
            SoundInfo sound = new SoundInfo { Name = node.RequireAttribute("name").Value };

            sound.Loop = node.TryAttribute<bool>("loop");

            sound.Volume = node.TryAttribute<float>("volume", 1);

            XAttribute pathattr = node.Attribute("path");
            XAttribute trackAttr = node.Attribute("track");
            if (pathattr != null)
            {
                sound.Type = AudioType.Wav;
                sound.Path = FilePath.FromRelative(pathattr.Value, project.BaseDir);
            }
            else if (trackAttr != null)
            {
                sound.Type = AudioType.NSF;

                int track;
                if (!trackAttr.Value.TryParse(out track) || track <= 0) throw new GameXmlException(trackAttr, "Sound track attribute must be an integer greater than zero.");
                sound.NsfTrack = track;

                sound.Priority = node.TryAttribute<byte>("priority", 100);
            }
            else
            {
                sound.Type = AudioType.Unknown;
            }

            project.AddSound(sound);
        }
    }
}
