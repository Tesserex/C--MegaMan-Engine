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

        public FilePath MusicNsf { get; set; }
        public FilePath EffectsNsf { get; set; }

        public HandlerTransfer StartHandler { get; set; }

        #endregion

        #region Loaded Resources

        private List<SoundInfo> sounds = new List<SoundInfo>();
        public IEnumerable<SoundInfo> Sounds { get { return sounds.AsReadOnly(); } }

        public void AddSound(SoundInfo sound)
        {
            sounds.Add(sound);
        }

        public void RemoveSound(SoundInfo sound)
        {
            sounds.Remove(sound);
        }

        private List<SceneInfo> scenes = new List<SceneInfo>();
        public IEnumerable<SceneInfo> Scenes { get { return scenes.AsReadOnly(); } }

        public void AddScene(SceneInfo scene)
        {
            scenes.Add(scene);
        }

        public void RemoveScene(SceneInfo scene)
        {
            scenes.Remove(scene);
        }

        private List<MenuInfo> menus = new List<MenuInfo>();
        public IEnumerable<MenuInfo> Menus { get { return menus.AsReadOnly(); } }

        public void AddMenu(MenuInfo menu)
        {
            menus.Add(menu);
        }

        public void RemoveMenu(MenuInfo menu)
        {
            menus.Remove(menu);
        }

        private List<FontInfo> fonts = new List<FontInfo>();
        public IEnumerable<FontInfo> Fonts { get { return fonts.AsReadOnly(); } }

        public void AddFont(FontInfo font)
        {
            fonts.Add(font);
        }

        public void RemoveFont(FontInfo font)
        {
            fonts.Remove(font);
        }

        private List<PaletteInfo> palettes = new List<PaletteInfo>();
        public IEnumerable<PaletteInfo> Palettes { get { return palettes.AsReadOnly(); } }

        public void AddPalette(PaletteInfo palette)
        {
            palettes.Add(palette);
        }

        public void RemovePalette(PaletteInfo palette)
        {
            palettes.Remove(palette);
        }

        private List<EntityInfo> entities = new List<EntityInfo>();
        public IEnumerable<EntityInfo> Entities { get { return entities.AsReadOnly(); } }

        public void AddEntity(EntityInfo entity)
        {
            entities.Add(entity);
        }

        public void RemoveEntity(EntityInfo entity)
        {
            entities.Remove(entity);
        }

        private List<EffectInfo> functions = new List<EffectInfo>();
        public IEnumerable<EffectInfo> Functions { get { return functions.AsReadOnly(); } }

        public void AddFunction(EffectInfo effect)
        {
            functions.Add(effect);
        }

        private Dictionary<string, TileProperties> entityProperties = new Dictionary<string, TileProperties>();
        public IDictionary<string, TileProperties> EntityProperties { get { return entityProperties; } }

        public void AddEntityProperties(TileProperties properties)
        {
            entityProperties[properties.Name] = properties;
        }

        public void RemoveEntityProperties(TileProperties properties)
        {
            entityProperties.Remove(properties.Name);
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
