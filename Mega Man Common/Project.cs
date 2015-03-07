using System.Collections.Generic;
using System.IO;
using System.Linq;
using MegaMan.Common.Entities;

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

        private List<FilePath> includeFolders = new List<FilePath>();
        private List<FilePath> includeFilesFromFolders = new List<FilePath>();
        private List<FilePath> includeFiles = new List<FilePath>();

        public IEnumerable<StageLinkInfo> Stages
        {
            get { return stages.AsReadOnly(); }
        }

        public void AddStage(StageLinkInfo stage)
        {
            this.stages.Add(stage);
        }

        public IEnumerable<FilePath> IncludeFiles
        {
            get { return includeFiles.AsReadOnly(); }
        }

        public IEnumerable<FilePath> IncludeFolders
        {
            get { return includeFolders.AsReadOnly(); }
        }

        public IEnumerable<FilePath> Includes
        {
            get { return includeFiles.Concat(includeFilesFromFolders).Distinct(); }
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

        public FilePath GameFile { get; set; }
        public string BaseDir
        {
            get
            {
                return GameFile.BasePath;
            }
        }

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

        private List<FontInfo> _fonts = new List<FontInfo>();
        public IEnumerable<FontInfo> Fonts { get { return _fonts.AsReadOnly(); } }

        public void AddFont(FontInfo font)
        {
            _fonts.Add(font);
        }

        public void RemoveFont(FontInfo font)
        {
            _fonts.Remove(font);
        }

        private List<PaletteInfo> _palettes = new List<PaletteInfo>();
        public IEnumerable<PaletteInfo> Palettes { get { return _palettes.AsReadOnly(); } }

        public void AddPalette(PaletteInfo palette)
        {
            _palettes.Add(palette);
        }

        public void RemovePalette(PaletteInfo palette)
        {
            _palettes.Remove(palette);
        }

        private List<EntityInfo> _entities = new List<EntityInfo>();
        public IEnumerable<EntityInfo> Entities { get { return _entities.AsReadOnly(); } }

        public void AddEntity(EntityInfo entity)
        {
            _entities.Add(entity);
        }

        public void RemoveEntity(EntityInfo entity)
        {
            _entities.Remove(entity);
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
            includeFiles.Add(FilePath.FromRelative(includePath, this.BaseDir));
        }

        public void AddIncludeFiles(IEnumerable<string> includePaths)
        {
            includeFiles.AddRange(includePaths.Select(p => FilePath.FromRelative(p, this.BaseDir)));
        }

        public void AddIncludeFolder(string includePath)
        {
            AddIncludeFolders(new List<string>() { includePath });
        }

        public void AddIncludeFolders(IEnumerable<string> includePaths)
        {
            var folderPaths = includePaths.Select(p => FilePath.FromRelative(p, this.BaseDir));
            includeFolders.AddRange(folderPaths);

            includeFilesFromFolders.AddRange(folderPaths
                .SelectMany(f => Directory.EnumerateFiles(
                    f.Absolute, "*.xml", SearchOption.AllDirectories)
                ).Select(p => FilePath.FromRelative(p, this.BaseDir)));
        }

        public void RemoveInclude(string includePath)
        {
            includeFolders.RemoveAll(f => f.Absolute == includePath);
            includeFiles.RemoveAll(f => f.Absolute == includePath);
        }
    }
}
