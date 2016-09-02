using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.AppData
{
    [Serializable]
    public class StoredAppData
    {
        public List<RecentProject> RecentProjects { get; set; }
        public string EngineAbsolutePath { get; set; }

        public StoredAppData()
        {
            RecentProjects = new List<RecentProject>();
        }

        public void AddRecentProject(ProjectDocument project)
        {
            var path = project.Project.GameFile.Absolute;
            var existing = RecentProjects.FirstOrDefault(p => p.AbsolutePath == path);

            if (existing != null)
            {
                RecentProjects.Remove(existing);
                RecentProjects.Insert(0, existing);
            }
            else
            {
                RecentProjects.Insert(0, new RecentProject() { Name = project.Name, AbsolutePath = path });
            }
        }

        public static StoredAppData Load()
        {
            var file = GetFilePath();

            if (!File.Exists(file))
                return new StoredAppData();

            var serializer = new XmlSerializer(typeof(StoredAppData));

            using (var reader = XmlReader.Create(file))
            {
                var data = serializer.Deserialize(reader);
                return (StoredAppData)data;
            }
        }

        public void Save()
        {
            var file = GetFilePath();
            var settings = new XmlWriterSettings() {
                Indent = true,
                NewLineChars = "\t",
                OmitXmlDeclaration = true
            };
            var serializer = new XmlSerializer(typeof(StoredAppData));

            using (var writer = XmlWriter.Create(file, settings))
            {
                serializer.Serialize(writer, this);
            }
        }

        private static string GetFilePath(string settingFilePath = null)
        {
            if (settingFilePath == null) settingFilePath = Constants.Paths.SettingFile;
            var file = Path.Combine(GetDirectory(), settingFilePath);
            return file;
        }

        public static string GetDirectory()
        {
            var data = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var directory = Path.Combine(data, "MegaManEngine");

            Directory.CreateDirectory(directory);
            return directory;
        }
    }
}
