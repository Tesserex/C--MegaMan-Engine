using System.Collections.Generic;
using System.Linq;
using MegaMan.Common.Entities;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common.IncludedObjects;

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
        private List<FilePath> includeFiles = new List<FilePath>();

        public IEnumerable<StageLinkInfo> Stages
        {
            get { return stages.AsReadOnly(); }
        }

        public void AddStage(StageLinkInfo stage)
        {
            stages.Add(stage);
        }

        public IEnumerable<FilePath> IncludeFiles
        {
            get { return includeFiles.AsReadOnly(); }
        }

        public IEnumerable<FilePath> IncludeFolders
        {
            get { return includeFolders.AsReadOnly(); }
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

        private List<EffectInfo> _functions = new List<EffectInfo>();
        public IEnumerable<EffectInfo> Functions { get { return _functions.AsReadOnly(); } }

        public void AddFunction(EffectInfo effect)
        {
            _functions.Add(effect);
        }

        private Dictionary<string, TileProperties> _entityProperties = new Dictionary<string, TileProperties>();
        public IDictionary<string, TileProperties> EntityProperties { get { return _entityProperties; } }

        public void AddEntityProperties(TileProperties properties)
        {
            _entityProperties[properties.Name] = properties;
        }

        public void RemoveEntityProperties(TileProperties properties)
        {
            _entityProperties.Remove(properties.Name);
        }

        #endregion

        public Project()
        {
            // sensible defaults where possible
            ScreenWidth = 256;
            ScreenHeight = 224;
        }

        public void AddIncludeFiles(IEnumerable<string> includePaths)
        {
            includeFiles.AddRange(includePaths.Select(p => FilePath.FromRelative(p, BaseDir)));
        }

        public void AddIncludeFolder(string includePath)
        {
            AddIncludeFolders(new List<string> { includePath });
        }

        public void AddIncludeFolders(IEnumerable<string> includePaths)
        {
            var folderPaths = includePaths.Select(p => FilePath.FromRelative(p, BaseDir));
            includeFolders.AddRange(folderPaths);
        }

        public void RemoveInclude(string includePath)
        {
            includeFolders.RemoveAll(f => f.Absolute == includePath);
            includeFiles.RemoveAll(f => f.Absolute == includePath);
        }
    }
}
