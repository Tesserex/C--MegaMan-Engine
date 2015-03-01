using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using MegaMan.Editor.Bll.Tools;

namespace MegaMan.Editor.Bll
{
    public class TilesetDocument
    {
        public Tileset Tileset { get; private set; }

        private List<MultiTileBrush> _brushes;
        public IEnumerable<MultiTileBrush> Brushes
        {
            get
            {
                return _brushes.AsReadOnly();
            }
        }

        public TilesetDocument(Tileset tileset)
        {
            Tileset = tileset;
            _brushes = new List<MultiTileBrush>();
            LoadBrushes();
        }

        public void AddBlockProperty()
        {
            Tileset.AddProperties(new TileProperties() {
                Name = "Block",
                Blocking = true,
                ResistX = 0.5f
            });
        }

        public void AddSpikeProperty()
        {
            Tileset.AddProperties(new TileProperties() {
                Name = "Spike",
                Blocking = true,
                Lethal = true
            });
        }

        public void AddLadderProperty()
        {
            Tileset.AddProperties(new TileProperties() {
                Name = "Ladder",
                ResistX = 0.5f,
                Climbable = true
            });
        }

        public void AddWaterProperty()
        {
            Tileset.AddProperties(new TileProperties() {
                Name = "Water",
                GravityMult = 0.4f
            });
        }

        public void AddConveyorRightProperty()
        {
            Tileset.AddProperties(new TileProperties() {
                Name = "Right Conveyor",
                ResistX = 0.5f,
                PushX = 0.1f
            });
        }

        public void AddConveyorLeftProperty()
        {
            Tileset.AddProperties(new TileProperties() {
                Name = "Left Conveyor",
                ResistX = 0.5f,
                PushX = -0.1f
            });
        }

        public void AddIceProperty()
        {
            Tileset.AddProperties(new TileProperties() {
                Name = "Ice",
                ResistX = 0.95f,
                DragX = 0.5f
            });
        }

        public void AddSandProperty()
        {
            Tileset.AddProperties(new TileProperties() {
                Name = "Quicksand",
                ResistX = 0.2f,
                DragX = 0.2f,
                GravityMult = 3,
                Sinking = 0.2f
            });
        }

        public void AddBrush(MultiTileBrush brush)
        {
            _brushes.Add(brush);
        }

        public void Save()
        {
            this.Tileset.Save(this.Tileset.FilePath.Absolute);
            SaveBrushes();
        }

        private void SaveBrushes()
        {
            string path = GetBrushFilePath();

            using (var stream = new System.IO.StreamWriter(path, false))
            {
                foreach (var brush in Brushes)
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

        private string GetBrushFilePath()
        {
            string dir = System.IO.Path.GetDirectoryName(Tileset.FilePath.Absolute);
            string file = System.IO.Path.GetFileNameWithoutExtension(Tileset.FilePath.Absolute);
            string path = System.IO.Path.Combine(dir, file + "_brushes.xml");
            return path;
        }

        private void LoadBrushes()
        {
            var path = GetBrushFilePath();

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
                        if (id >= 0) brush.AddTile(Tileset[id], x, y);

                        y++;
                        if (y >= brush.Height)
                        {
                            y = 0;
                            x++;
                        }
                    }

                    _brushes.Add(brush);
                }
            }
        }
    }
}
