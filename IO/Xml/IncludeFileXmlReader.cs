using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;
using Ninject;

namespace MegaMan.IO.Xml
{
    internal class IncludeFileXmlReader
    {
        private Project _project;

        private Dictionary<string, IIncludeXmlReader> _readers;

        public IncludeFileXmlReader()
        {
            _readers = Assembly.GetAssembly(typeof(IIncludeXmlReader))
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IIncludeXmlReader)))
                .Select(t => Injector.Container.Get(t))
                .Cast<IIncludeXmlReader>()
                .ToDictionary(r => r.NodeName);
        }

        public void LoadIncludedFile(Project project, FilePath filePath, Stream stream, IDataSource dataSource)
        {
            _project = project;

            try
            {
                XDocument document = XDocument.Load(stream, LoadOptions.SetLineInfo);
                foreach (XElement element in document.Elements())
                {
                    if (_readers.ContainsKey(element.Name.LocalName))
                    {
                        var obj = _readers[element.Name.LocalName].Load(project, element, dataSource);
                        obj.StoragePath = filePath;
                    }
                }
            }
            catch (GameXmlException ex)
            {
                ex.File = filePath.Absolute;
                throw;
            }
        }

        public static SoundInfo LoadSound(XElement soundNode, string basePath)
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
    }
}
