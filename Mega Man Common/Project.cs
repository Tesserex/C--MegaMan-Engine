using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public class StageLinkInfo
    {
        public string Name { get; set; }
        public FilePath StagePath { get; set; }
        public HandlerTransfer WinHandler { get; set; }
        public HandlerTransfer LoseHandler { get; set; }
    }

    public class Project
    {
        #region Game XML File Stuff

        private List<StageLinkInfo> stages = new List<StageLinkInfo>();

        private List<string> includeFolders = new List<string>();
        private List<string> includeFilesFromFolders = new List<string>();
        private List<string> includeFiles = new List<string>();

        public IEnumerable<StageLinkInfo> Stages
        {
            get { return stages.AsReadOnly(); }
        }

        public void AddStage(StageLinkInfo stage)
        {
            this.stages.Add(stage);
        }

        public IEnumerable<string> IncludeFiles
        {
            get { return includeFiles.AsReadOnly(); }
        }

        public IEnumerable<string> IncludeFolders
        {
            get { return includeFolders.AsReadOnly(); }
        }

        public IEnumerable<string> Includes
        {
            get { return includeFiles.Concat(includeFilesFromFolders); }
        }

        public string Name
        {
            get;
            set;
        }

        public string Author
        {
            get;
            set;
        }

        public string GameFile { get; set; }
        public string BaseDir { get; set; }

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public FilePath MusicNSF { get; set; }
        public FilePath EffectsNSF { get; set; }

        public HandlerTransfer StartHandler { get; set; }

        #endregion

        #region Loaded Resources

        private List<SoundInfo> _sounds = new List<SoundInfo>();
        public IEnumerable<SoundInfo> Sounds { get { return _sounds.AsReadOnly(); } }

        public void AddSound(SoundInfo sound)
        {
            _sounds.Add(sound);
        }

        public void RemoveSound(SoundInfo sound)
        {
            _sounds.Remove(sound);
        }

        private List<SceneInfo> _scenes = new List<SceneInfo>();
        public IEnumerable<SceneInfo> Scenes { get { return _scenes.AsReadOnly(); } }

        public void AddScene(SceneInfo scene)
        {
            _scenes.Add(scene);
        }

        public void RemoveScene(SceneInfo scene)
        {
            _scenes.Remove(scene);
        }

        private List<MenuInfo> _menus = new List<MenuInfo>();
        public IEnumerable<MenuInfo> Menus { get { return _menus.AsReadOnly(); } }

        public void AddMenu(MenuInfo menu)
        {
            _menus.Add(menu);
        }

        public void RemoveMenu(MenuInfo menu)
        {
            _menus.Remove(menu);
        }

        #endregion

        public Project()
        {
            // sensible defaults where possible
            ScreenWidth = 256;
            ScreenHeight = 224;
        }

        public void AddIncludeFile(string includePath)
        {
            includeFiles.Add(includePath);
        }

        public void AddIncludeFiles(IEnumerable<string> includePaths)
        {
            includeFiles.AddRange(includePaths);
        }

        public void AddIncludeFolder(string includePath)
        {
            AddIncludeFolders(new List<string>() { includePath });
        }

        public void AddIncludeFolders(IEnumerable<string> includePaths)
        {
            includeFolders.AddRange(includePaths);

            includeFilesFromFolders.AddRange(includePaths
                .SelectMany(f => Directory.EnumerateFiles(
                    Path.Combine(BaseDir, f), "*.xml", SearchOption.AllDirectories)
                ));
        }
    }
}
