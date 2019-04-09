using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Tools;
using MegaMan.IO;

namespace MegaMan.Editor.Services
{
    public class DataAccessService : IDataAccessService
    {
        private readonly IWriterProvider _writerProvider;
        private readonly IGameLoader _gameLoader;
        public IReaderProvider Reader { get; private set; }

        public DataAccessService(IGameLoader gameLoader, IWriterProvider writerProvider)
        {
            _gameLoader = gameLoader;
            _writerProvider = writerProvider;
        }

        public ProjectDocument CreateProject(string directory)
        {
            var project = new Project {
                GameFile = FilePath.FromRelative("game.xml", directory)
            };

            var p = new ProjectDocument(new ProjectFileStructure(project), project, this);
            return p;
        }

        public ProjectDocument LoadProject(string filePath)
        {
            Reader = _gameLoader.Load(filePath);
            var project = Reader.GetProjectReader().Load();
            var structure = new ProjectFileStructure(project);
            var projectDocument = new ProjectDocument(structure, project, this);
            return projectDocument;
        }

        public void SaveProject(ProjectDocument project)
        {
            var projectWriter = _writerProvider.GetProjectWriter();
            projectWriter.Save(project.Project);
            project.Dirty = false;

            foreach (var stage in project.Stages)
            {
                SaveStage(stage);
            }

            var allEntities = project.Entities.Concat(project.UnloadedEntities).ToList();

            foreach (var entity in allEntities)
            {
                if (entity.StoragePath == null)
                    entity.StoragePath = project.FileStructure.CreateEntityPath(entity.Name);
            }

            var entityFileGroups = allEntities
                .GroupBy(e => e.StoragePath.Absolute);

            foreach (var group in entityFileGroups)
            {
                SaveEntities(group, group.Key);
            }
        }

        public void ExportProject(ProjectDocument project)
        {
            SaveProject(project);


        }

        public StageDocument LoadStage(ProjectDocument project, StageLinkInfo linkInfo)
        {
            var reader = Reader.GetStageReader(linkInfo.StagePath);
            var stage = reader.Load(linkInfo.StagePath);
            var document = new StageDocument(project, stage, linkInfo);
            return document;
        }

        public void SaveStage(StageDocument stage)
        {
            var stageWriter = _writerProvider.GetStageWriter();
            stageWriter.Save(stage.Info);
            stage.Dirty = false;
            SaveTileset(stage.Tileset);
        }

        public TilesetDocument LoadTileset(FilePath filePath)
        {
            var tilesetReader = Reader.GetTilesetReader(filePath);
            var tileset = tilesetReader.Load(filePath);
            var tilesetDocument = new TilesetDocument(tileset);
            return tilesetDocument;
        }

        public TilesetDocument CreateTileset(FilePath filePath)
        {
            var tileset = new Tileset {
                FilePath = filePath,
                TileSize = 16
            };

            var document = new TilesetDocument(tileset);
            AddDefaultTileProperties(document);

            return document;
        }

        private void AddDefaultTileProperties(TilesetDocument tileset)
        {
            tileset.AddBlockProperty();
            tileset.AddSpikeProperty();
            tileset.AddLadderProperty();
            tileset.AddWaterProperty();
        }

        public void SaveTileset(TilesetDocument tileset)
        {
            var tilesetWriter = _writerProvider.GetTilesetWriter();
            tilesetWriter.Save(tileset.Tileset);
            SaveBrushes(tileset);

            if (tileset.IsSheetDirty)
            {
                var sheet = SpriteBitmapCache.GetOrLoadImage(tileset.SheetPath.Absolute);

                using (var fileStream = new FileStream(tileset.SheetPath.Absolute, FileMode.OpenOrCreate))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(sheet));
                    encoder.Save(fileStream);
                }

                tileset.IsSheetDirty = false;
            }
        }

        private void SaveBrushes(TilesetDocument tileset)
        {
            string path = GetBrushFilePath(tileset);

            using (var stream = new StreamWriter(path, false))
            {
                foreach (var brush in tileset.Brushes)
                {
                    stream.Write(brush.Width);
                    stream.Write(' ');
                    stream.Write(brush.Height);
                    foreach (var cell in brush.Cells.SelectMany(a => a))
                    {
                        stream.Write(' ');
                        if (cell.tile == null) stream.Write(UnknownTile.UnknownId);
                        else stream.Write(cell.tile.Id);
                    }
                    stream.WriteLine();
                }
            }
        }

        private string GetBrushFilePath(TilesetDocument tileset)
        {
            string dir = Path.GetDirectoryName(tileset.Tileset.FilePath.Absolute);
            string file = Path.GetFileNameWithoutExtension(tileset.Tileset.FilePath.Absolute);
            string path = Path.Combine(dir, file + "_brushes.xml");
            return path;
        }

        private void LoadBrushes(TilesetDocument tileset)
        {
            var path = GetBrushFilePath(tileset);

            if (!File.Exists(path)) return;

            using (var stream = new StreamReader(path))
            {
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    if (line == null) break;

                    string[] info = line.Split(' ');

                    var brush = new MultiTileBrush(int.Parse(info[0]), int.Parse(info[1]));

                    int x = 0; int y = 0;
                    for (int i = 2; i < info.Length; i++)
                    {
                        int id = int.Parse(info[i]);
                        if (id >= 0) brush.AddTile(tileset.Tileset[id], x, y);

                        y++;
                        if (y >= brush.Height)
                        {
                            y = 0;
                            x++;
                        }
                    }

                    tileset.AddBrush(brush);
                }
            }
        }

        private void SaveEntities(IEnumerable<EntityInfo> entities, string path)
        {
            var writer = _writerProvider.GetEntityGroupWriter();
            writer.Write(entities, path);
        }

        public void SaveEntity(EntityInfo entity, string path)
        {
            var writer = _writerProvider.GetEntityWriter();
            writer.Write(entity, path);
        }
    }
}
