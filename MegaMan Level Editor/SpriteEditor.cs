using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MegaMan.Common;

namespace MegaMan.LevelEditor
{
    public partial class SpriteEditor : Form
    {
        private Project project;
        private Image tileSheet;
        private Image transparency;
        private Timer timer;

        private bool snap;
        private int spriteWidth, spriteHeight;
        private Point highlight;
        private bool highlightOn;

        public event Action SpriteChange;

        private Sprite sprite;
        public Sprite Sprite
        {
            get { return sprite; }
            set
            {
                sprite = value;
                SetupSheet();
                SetupLayout();

                frameTotalLabel.Text = "of " + sprite.Count.ToString();
                currentFrame.Minimum = 1;
                currentFrame.Maximum = sprite.Count;

                textWidth.Text = sprite.Width.ToString();
                textHeight.Text = sprite.Height.ToString();

                sprite.Play();
            }
        }

        public SpriteEditor(Project project)
        {
            InitializeComponent();
            this.project = project;

            snap = true;
            spriteWidth = spriteHeight = 32;
            textWidth.Text = textHeight.Text = "32";

            InitSprite(32, 32);

            timer = new Timer();
            timer.Interval = 17;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.Sprite != null) this.Sprite.Update();
            DrawPreview();
        }

        private void DrawPreview()
        {
            if (spritePreview.Image == null || Sprite.Count == 0) return;

            using (Graphics g = Graphics.FromImage(spritePreview.Image))
            {
                g.Clear(spritePreview.BackColor);
                Sprite.Draw(g, 0, 0);
            }
            spritePreview.Refresh();
        }

        private void InitSprite(int width, int height)
        {
            string sheetpath = null;
            if (Sprite != null && Sprite.SheetPath != null) sheetpath = Sprite.SheetPath.Absolute;

            sprite = new Sprite(width, height);
            spriteWidth = width;
            spriteHeight = height;
            if (sheetpath != null) sprite.SheetPath = FilePath.FromAbsolute(sheetpath, project.BaseDir);

            frameTotalLabel.Text = "of " + sprite.Count.ToString();
            currentFrame.Minimum = 1;
            currentFrame.Maximum = sprite.Count;
            duration.Value = 0;

            textWidth.Text = sprite.Width.ToString();
            textHeight.Text = sprite.Height.ToString();

            InitPreview();

            sprite.Play();

            if (SpriteChange != null) SpriteChange();
        }

        private void InitPreview()
        {
            if (Sprite.Sheet == null) return;

            spritePreview.Width = Sprite.Width;
            spritePreview.Height = Sprite.Height;
            Bitmap prev = new Bitmap(Sprite.Width, Sprite.Height);
            prev.SetResolution(Sprite.Sheet.HorizontalResolution, Sprite.Sheet.VerticalResolution);
            spritePreview.Image = prev;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            SetupLayout();
        }

        private void SetupLayout()
        {
            // adjust sizes of components
            groupSheet.Width = this.Width - 276;

            if (tileSheet != null)
            {
                imagePanel.Width = Math.Min(tileSheet.Width, groupSheet.Width - (groupSheet.Left * 2) - 2);
                imagePanel.Height = Math.Min(tileSheet.Height, groupSheet.Height - imagePanel.Top - 6);
            }

            spritePreview.Left = groupSheet.Right + 10;
            spritePreview.Top = groupFrame.Bottom + 10;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (sourceImage.Image != null) sourceImage.Dispose();
            if (transparency != null) transparency.Dispose();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Choose a Tile Sheet";
            dialog.Filter = "Image Files (bmp, jpg, png, gif)|*.png;*.gif;*.jpg;*.bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filename = dialog.FileName;

                if (tileSheet != null) tileSheet.Dispose();
                if (transparency != null) transparency.Dispose();
                if (sourceImage.Image != null) sourceImage.Image.Dispose();

                this.Sprite.SheetPath = FilePath.FromAbsolute(filename, project.BaseDir);

                SetupSheet();
            }
        }

        private void SetupSheet()
        {
            tileSheet = this.Sprite.Sheet;

            Bitmap srcImg = new Bitmap(tileSheet.Width, tileSheet.Height);
            transparency = new Bitmap(tileSheet.Width, tileSheet.Height);
            srcImg.SetResolution(tileSheet.HorizontalResolution, tileSheet.VerticalResolution);
            sourceImage.Image = srcImg;
            sourceImage.Size = sourceImage.Image.Size;
            imagePanel.Size = sourceImage.Size;

            CreateTransparencyLayer();

            InitPreview();

            ReDrawSource();
        }

        private void CreateTransparencyLayer()
        {
            using (Graphics g = Graphics.FromImage(transparency))
            {
                int x = 0;
                int y = 0;
                Brush fill;
                while (y < transparency.Height)
                {
                    fill = (y % 16 == 0) ? Brushes.White : Brushes.LightGray;
                    while (x < transparency.Width)
                    {
                        g.FillRectangle(fill, x, y, 8, 8);
                        fill = (fill == Brushes.White) ? Brushes.LightGray : Brushes.White;
                        x += 8;
                    }
                    x = 0;
                    y += 8;
                }
            }
        }

        private void ReDrawSource()
        {
            if (tileSheet == null) return;

            using (Graphics g = Graphics.FromImage(sourceImage.Image))
            {
                g.DrawImage(transparency, 0, 0);
                g.DrawImage(tileSheet, 0, 0);

                if (highlightOn)
                {
                    g.DrawRectangle(Pens.DarkGreen, highlight.X, highlight.Y, spriteWidth, spriteHeight);
                }
            }

            sourceImage.Refresh();
        }

        private void checkSnap_CheckedChanged(object sender, EventArgs e)
        {
            this.snap = checkSnap.Checked;
        }

        private void sourceImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (spriteHeight == 0 || spriteWidth == 0) return;

            if (snap)
            {
                highlight.X = (e.X / spriteWidth) * spriteWidth;
                highlight.Y = (e.Y / spriteHeight) * spriteHeight;
            }
            else
            {
                highlight.X = e.X - (spriteWidth / 2);
                highlight.Y = e.Y - (spriteHeight / 2);
            }

            ReDrawSource();
        }

        private void sourceImage_MouseEnter(object sender, EventArgs e)
        {
            highlightOn = true;
        }

        private void sourceImage_MouseLeave(object sender, EventArgs e)
        {
            highlightOn = false;
            ReDrawSource();
        }

        private void sourceImage_Click(object sender, EventArgs e)
        {
            if (tileSheet != null && Sprite.Count > 0 && Sprite.Count >= currentFrame.Value) Sprite[(int)currentFrame.Value - 1].SetSheetPosition(new Rectangle(highlight, new Size(spriteWidth, spriteHeight)));
        }

        private void buttonAddFrame_Click(object sender, EventArgs e)
        {
            this.Sprite.AddFrame();
            frameTotalLabel.Text = "of " + Sprite.Count.ToString();
            currentFrame.Minimum = 1;
            currentFrame.Maximum = Sprite.Count;
            if (currentFrame.Value < currentFrame.Minimum) currentFrame.Value = currentFrame.Minimum;
        }

        private void buttonRefreshSize_Click(object sender, EventArgs e)
        {
            int width, height;
            if (!int.TryParse(textWidth.Text, out width) || width <= 0)
            {
                MessageBox.Show("Width must be a whole number greater than zero.", "MegaMan Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(textHeight.Text, out height) || height <= 0)
            {
                MessageBox.Show("Height must be a whole number greater than zero.", "MegaMan Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            InitSprite(width, height);
        }

        private void radioWhite_CheckedChanged(object sender, EventArgs e)
        {
            if (radioWhite.Checked)
            {
                radioBlack.Checked = false;
                spritePreview.BackColor = Color.White;
                DrawPreview();
            }
        }

        private void radioBlack_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBlack.Checked)
            {
                radioWhite.Checked = false;
                spritePreview.BackColor = Color.Black;
                DrawPreview();
            }
        }

        private void currentFrame_ValueChanged(object sender, EventArgs e)
        {
            if (Sprite.Count == 0 || Sprite.Count < currentFrame.Value) return;
            duration.Value = Sprite[(int)currentFrame.Value - 1].Duration;
        }

        private void duration_ValueChanged(object sender, EventArgs e)
        {
            if (tileSheet != null && Sprite.Count > 0 && Sprite.Count >= currentFrame.Value) Sprite[(int)currentFrame.Value - 1].Duration = (int)duration.Value;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Dispose();
            this.Close();
        }
    }
}
