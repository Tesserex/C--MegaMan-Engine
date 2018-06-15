using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class SoundCommandXmlWriter : ICommandXmlWriter
    {
        private readonly SoundXmlWriter soundWriter;

        public SoundCommandXmlWriter(SoundXmlWriter soundWriter)
        {
            this.soundWriter = soundWriter;
        }

        public Type CommandType
        {
            get
            {
                return typeof(SceneSoundCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var sound = (SceneSoundCommandInfo)info;
            soundWriter.Write(sound.SoundInfo, writer);
        }
    }
}
