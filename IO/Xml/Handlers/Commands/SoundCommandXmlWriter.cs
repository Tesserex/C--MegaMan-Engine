using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class SoundCommandXmlWriter : ICommandXmlWriter
    {
        private readonly SoundXmlWriter _soundWriter;

        public SoundCommandXmlWriter(SoundXmlWriter soundWriter)
        {
            _soundWriter = soundWriter;
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
            _soundWriter.Write(sound.SoundInfo, writer);
        }
    }
}
