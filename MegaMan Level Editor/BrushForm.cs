using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MegaMan.Common;

namespace MegaMan.LevelEditor
{
    public partial class BrushForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private readonly Dictionary<string, List<ITileBrush>> brushSets = new Dictionary<string, List<ITileBrush>>();
        private List<ITileBrush> brushes;
        private Tileset Tileset;
        private ITileBrush currentBrush;
        private TileBrush creatingBrush;
        private readonly Dictionary<ITileBrush, Panel> brushPanels = new Dictionary<ITileBrush, Panel>();

        public BrushForm()
        {
            InitializeComponent();
            splitter.Panel1Collapsed = true;
        }

        public void ChangeTileset(Tileset tileset)
        {
            Clear();
            Tileset = tileset;
            if (tileset == null) brushes = null;
            else if (brushSets.ContainsKey(tileset.FilePath))
            {
                brushes = brushSets[tileset.FilePath];
            }
            else
            {
                brushes = new List<ITileBrush>();
                brushSets.Add(tileset.FilePath, brushes);
                LoadBrushes();
            }

            if (brushes != null)
            {
                foreach (var brush in brushes) AddBrushPanel(brush);
            }
        }

        public void Clear()
        {
            brushPanels.Clear();
            brushPanel.Controls.Clear();
            Tileset = null;
        }

        public event BrushChangedHandler BrushChanged;

        private void SaveBrushes()
        {
            string dir = System.IO.Path.GetDirectoryName(Tileset.FilePath);
            string file = System.IO.Path.GetFileNameWithoutExtension(Tileset.FilePath);
            string path = System.IO.Path.Combine(dir, file + "_brushes.xml");

            using (var stream = new System.IO.StreamWriter(path, false))
            {
                foreach (var brush in brushes)
                {
                    stream.Write(brush.Width);
                    stream.Write(' ');
                    stream.Write(brush.Height);
                    foreach (var cell in brush.Cells())
                    {
                        stream.Write(' ');
                        if (cell.tile == null) stream.Write(-1);
                        else stream.Write(cell.tile.Id);
                    }
                    stream.WriteLine();
                }
            }
        }

        private void LoadBrushes()
        {
            string dir = System.IO.Path.GetDirectoryName(Tileset.FilePath);
            string file = System.IO.Path.GetFileNameWithoutExtension(Tileset.FilePath);
            string path = System.IO.Path.Combine(dir, file + "_brushes.xml");

            if (!System.IO.File.Exists(path)) return;

            using (var stream = new System.IO.StreamReader(path))
            {
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    if (line == null) break;

                    string[] info = line.Split(' ');

                    TileBrush brush = new TileBrush(int.Parse(info[0]), int.Parse(info[1]));

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

                    brushes.Add(brush);
                }
            }
        }

        private void buttonNewBrush_Click(object sender, EventArgs e)
        {
            if (Tileset == null || brushes == null) return;

            creatingBrush = new TileBrush(2, 2);

            splitter.Panel1Collapsed = false;

            ResetNewBrush(2, 2);
        }

        private void SaveNewBrush(object sender, EventArgs e)
        {
            AddBrush(creatingBrush);
            creatingBrush = null;
            splitter.Panel1Collapsed = true;
        }

        public void AddBrush(ITileBrush brush)
        {
            if (brush == null) return;

            brushes.Add(brush);
            AddBrushPanel(brush);
            SaveBrushes();
        }

        private void CancelNewBrush(object sender, EventArgs e)
        {
            creatingBrush = null;
            splitter.Panel1Collapsed = true;
        }

        private void brushPict_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.Instance.CurrentBrush == null) return;

            int tx = e.X / Tileset.TileSize;
            int ty = e.Y / Tileset.TileSize;

            foreach (TileBrushCell cell in MainForm.Instance.CurrentBrush.Cells())
            {
                creatingBrush.AddTile(cell.tile, cell.x + tx, cell.y + ty);
            }

            ReDraw();
        }

        private void ReDraw()
        {
            using (Graphics g = Graphics.FromImage(brushPict.Image))
            {
                g.Clear(Color.Black);
                creatingBrush.DrawOn(g, 0, 0);
            }
            brushPict.Refresh();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            int width;
            int height;
            if (!int.TryParse(widthBox.Text, out width)) return;
            if (!int.TryParse(heightBox.Text, out height)) return;

            ResetNewBrush(width, height);
        }

        private void ResetNewBrush(int width, int height)
        {
            creatingBrush.Reset(width, height);

            if (brushPict.Image != null) brushPict.Image.Dispose();

            brushPict.Image = new Bitmap(width * Tileset.TileSize, height * Tileset.TileSize);
            brushPict.Size = brushPict.Image.Size;
            ReDraw();
        }

        private void AddBrushPanel(ITileBrush brush)
        {
            PictureBox brushPict = new PictureBox();

            if (Tileset != null)
            {
                brushPict.Image = new Bitmap(brush.Width * Tileset.TileSize, brush.Height * Tileset.TileSize);
                brushPict.Size = brushPict.Image.Size;
                using (Graphics g = Graphics.FromImage(brushPict.Image))
                {
                    brush.DrawOn(g, 0, 0);
                }
            }

            Panel border = new Panel
            {
                BackColor = brushPanel.BackColor,
                Width = brushPict.Width + 8,
                Height = brushPict.Height + 8
            };

            border.Controls.Add(brushPict);
            brushPict.Top = 4;
            brushPict.Left = 4;

            brushPict.Click += (snd, args) =>
            {
                ChangeBrush(brush);
                foreach (Control c in brushPanel.Controls) c.BackColor = brushPanel.BackColor;
                border.BackColor = Color.Orange;
            };

            brushPanels.Add(brush, border);
            brushPanel.Controls.Add(border);
        }

        private void ChangeBrush(ITileBrush brush)
        {
            currentBrush = brush;
            BrushChangedEventArgs args = new BrushChangedEventArgs(brush);
            if (BrushChanged != null) BrushChanged(args);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (brushes == null || currentBrush == null) return;

            brushPanel.Controls.Remove(brushPanels[currentBrush]);
            brushes.Remove(currentBrush);
            SaveBrushes();
        }
    }

    public class BrushChangedEventArgs : EventArgs
    {
        public ITileBrush Brush { get; private set; }

        public BrushChangedEventArgs(ITileBrush brush)
        {
            Brush = brush;
        }
    }

    public delegate void BrushChangedHandler(BrushChangedEventArgs e);
}
