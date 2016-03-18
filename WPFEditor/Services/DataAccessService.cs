using System.Linq;
using MegaMan.Common.Entities;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Tools;
using MegaMan.IO;

namespace MegaMan.Editor.Services
{
    public class DataAccessService : IDataAccessService
    {
        private readonly IWriterProvider _writerProvider;

        public DataAccessService(IWriterProvider writerProvider)
        {
            _writerProvider = writerProvider;
        }

        public void SaveProject(ProjectDocument project)
        {
            var projectWriter = _writerProvider.GetProjectWriter();
            projectWriter.Save(project.Project);
            project.Dirty = false;

            foreach (var stageName in project.StageNames)
            {
                var stage = project.StageByName(stageName);
                SaveStage(stage);
            }

            foreach (var entity in project.Entities)
            {
                var entityPath = project.FileStructure.CreateEntityPath(entity.Name);
                SaveEntity(entity, entityPath.Absolute);
            }
        }

        public void SaveStage(StageDocument stage)
        {
            var stageWriter = _writerProvider.GetStageWriter();
            stageWriter.Save(stage.Info);
            stage.Dirty = false;
            SaveTileset(stage.Tileset);
        }

        public void SaveTileset(TilesetDocument tileset)
        {
            var tilesetWriter = _writerProvider.GetTilesetWriter();
            tilesetWriter.Save(tileset.Tileset);
            SaveBrushes(tileset);
        }

        private void SaveBrushes(TilesetDocument tileset)
        {
            string path = GetBrushFilePath(tileset);

            using (var stream = new System.IO.StreamWriter(path, false))
            {
                foreach (var brush in tileset.Brushes)
                {
                    stream.Write(brush.Width);
                    stream.Write(' ');
                    stream.Write(brush.Height);
                    foreach (var cell in brush.Cells.SelectMany(a => a))
                    {
                        stream.Write(' ');
                        if (cell.tile == null) stream.Write(-1);
                        else stream.Write(cell.tile.Id);
                    }
                    stream.WriteLine();
                }
            }
        }

        private string GetBrushFilePath(TilesetDocument tileset)
        {
            string dir = System.IO.Path.GetDirectoryName(tileset.Tileset.FilePath.Absolute);
            string file = System.IO.Path.GetFileNameWithoutExtension(tileset.Tileset.FilePath.Absolute);
            string path = System.IO.Path.Combine(dir, file + "_brushes.xml");
            return path;
        }

        private void LoadBrushes(TilesetDocument tileset)
        {
            var path = GetBrushFilePath(tileset);

            if (!System.IO.File.Exists(path)) return;

            using (var stream = new System.IO.StreamReader(path))
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

        public void SaveEntity(EntityInfo entity, string path)
        {
            var writer = _writerProvider.GetEntityWriter();
            writer.Write(entity, path);
        }
    }
}
