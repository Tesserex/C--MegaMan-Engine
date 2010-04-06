using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.Linq;

namespace Mega_Man
{
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private string settingsPath;

        public Form1()
        {
            InitializeComponent();
            xnaImage.Init();

            // look for config file
            try
            {
                LoadConfig();
            }
            catch
            {
                MessageBox.Show("The config file could was not loaded successfully.");
            }

            ResizeScreen(Const.PixelsAcross, Const.PixelsDown);

            Game.ScreenSizeChanged += new EventHandler<ScreenSizeChangedEventArgs>(Game_ScreenSizeChanged);
            Engine.Instance.GameLogicTick += new GameTickEventHandler(Instance_GameLogicTick);
            Engine.Instance.GameRenderBegin += new GameRenderEventHandler(GameRenderBegin);
            Engine.Instance.GameRenderEnd += new GameRenderEventHandler(Instance_GameRenderEnd);
        }

        protected override void OnClosed(EventArgs e)
        {
            // write settings
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(settingsPath, null);
            writer.Indentation = 1;
            writer.IndentChar = '\t';
            writer.Formatting = System.Xml.Formatting.Indented;

            writer.WriteStartElement("Settings");

            writer.WriteStartElement("Keys");

            writer.WriteStartElement("Up");
            writer.WriteValue(GameInputKeys.Up.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Down");
            writer.WriteValue(GameInputKeys.Down.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Left");
            writer.WriteValue(GameInputKeys.Left.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Right");
            writer.WriteValue(GameInputKeys.Right.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Jump");
            writer.WriteValue(GameInputKeys.Jump.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Shoot");
            writer.WriteValue(GameInputKeys.Shoot.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Start");
            writer.WriteValue(GameInputKeys.Start.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Select");
            writer.WriteValue(GameInputKeys.Select.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.Close();
            base.OnClosed(e);
        }

        private void LoadConfig()
        {
            settingsPath = System.IO.Path.Combine(Application.StartupPath, "settings.xml");
            if (System.IO.File.Exists(settingsPath))
            {
                XElement settings = XElement.Load(settingsPath);
                XElement keys = settings.Element("Keys");
                foreach (XElement node in keys.Elements())
                {
                    switch (node.Name.LocalName)
                    {
                        case "Up":
                            GameInputKeys.Up = (Keys)Enum.Parse(typeof(Keys), node.Value);
                            break;
                        case "Down":
                            GameInputKeys.Down = (Keys)Enum.Parse(typeof(Keys), node.Value);
                            break;
                        case "Left":
                            GameInputKeys.Left = (Keys)Enum.Parse(typeof(Keys), node.Value);
                            break;
                        case "Right":
                            GameInputKeys.Right = (Keys)Enum.Parse(typeof(Keys), node.Value);
                            break;
                        case "Jump":
                            GameInputKeys.Jump = (Keys)Enum.Parse(typeof(Keys), node.Value);
                            break;
                        case "Shoot":
                            GameInputKeys.Shoot = (Keys)Enum.Parse(typeof(Keys), node.Value);
                            break;
                        case "Start":
                            GameInputKeys.Start = (Keys)Enum.Parse(typeof(Keys), node.Value);
                            break;
                        case "Select":
                            GameInputKeys.Select = (Keys)Enum.Parse(typeof(Keys), node.Value);
                            break;
                    }
                }
            }
        }

        void Game_ScreenSizeChanged(object sender, ScreenSizeChangedEventArgs e)
        {
            ResizeScreen(e.PixelsAcross, e.PixelsDown);
        }

        private void ResizeScreen(int width, int height)
        {
            Bitmap b = new Bitmap(width, height);
            b.SetResolution(Const.Resolution, Const.Resolution);

            if (screenImage.Image != null) screenImage.Image.Dispose();
            screenImage.Image = b;
            if (graphics != null) graphics.Dispose();
            graphics = Graphics.FromImage(screenImage.Image);

            // tell the image not to get crushed by the form
            this.screenImage.Dock = DockStyle.None;
            // tell the form to fit the image
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.screenImage.Width = width;
            this.screenImage.Height = height;
            // now remember the form size
            int tempheight = this.Height;
            int tempwidth = this.Width;
            // now un-autosize to re-enable resizing
            this.AutoSize = false;
            this.AutoSizeMode = AutoSizeMode.GrowOnly;
            // reset the form size
            tempheight += debugBar.Height;
            this.Height = tempheight;
            this.Width = tempwidth + 256;
            // redock the image
            this.screenImage.Dock = DockStyle.Left;
        }

        void Instance_GameLogicTick(GameTickEventArgs e)
        {
            float fps = 1 / e.TimeElapsed;
            fpsLabel.Text = "FPS: " + fps.ToString("N2");
            thinkLabel.Text = "Busy: " + (Engine.Instance.ThinkTime * 100).ToString("N0") + "%";
            entityLabel.Text = "Entities: " + GameEntity.ActiveCount.ToString();
        }

        private void Instance_GameRenderEnd(GameRenderEventArgs e)
        {
            if (e.Opacity >= 1)
            {
                graphics.DrawImage(e.Layers.Background, 0, 0);
                for (int i = 0; i < e.Layers.Sprites.Length; i++) graphics.DrawImage(e.Layers.Sprites[i], 0, 0);
                graphics.DrawImage(e.Layers.Foreground, 0, 0);
            }
            else
            {
                System.Drawing.Imaging.ColorMatrix matrix = new System.Drawing.Imaging.ColorMatrix();
                matrix.Matrix33 = e.Opacity;
                System.Drawing.Imaging.ImageAttributes attr = new System.Drawing.Imaging.ImageAttributes();
                attr.SetColorMatrix(matrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                // flatten image first
                Bitmap flat = new Bitmap(e.Layers.Background.Width, e.Layers.Background.Height);
                flat.SetResolution(e.Layers.Background.HorizontalResolution, e.Layers.Background.VerticalResolution);
                using (Graphics g = Graphics.FromImage(flat))
                {
                    g.DrawImage(e.Layers.Background, 0, 0);
                    for (int i = 0; i < e.Layers.Sprites.Length; i++) g.DrawImage(e.Layers.Sprites[i], 0, 0);
                    g.DrawImage(e.Layers.Foreground, 0, 0);
                }

                Rectangle dest = new Rectangle(0, 0, flat.Width, flat.Height);
                graphics.DrawImage(flat, dest, 0, 0, flat.Width, flat.Height, GraphicsUnit.Pixel, attr);
                flat.Dispose();
                attr.Dispose();
            }
            screenImage.Refresh();
        }

        private void GameRenderBegin(GameRenderEventArgs e)
        {
            graphics.Clear(Color.Black);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    Game.Load(dialog.FileName);
                }
                catch (EntityXmlException ex)
                {
                    StringBuilder message = new StringBuilder("There is a syntax error in one of your game files.\n\n");
                    if (ex.File != null) message.Append("File: ").Append(ex.File).Append('\n');
                    if (ex.Line != 0) message.Append("Line: ").Append(ex.Line.ToString()).Append('\n');
                    if (ex.Entity != null) message.Append("Entity: ").Append(ex.Entity).Append('\n');
                    if (ex.Tag != null) message.Append("Tag: ").Append(ex.Tag).Append('\n');
                    if (ex.Attribute != null) message.Append("Attribute: ").Append(ex.Attribute).Append('\n');

                    message.Append("\n").Append(ex.Message);

                    MessageBox.Show(message.ToString(), "Game Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Game.CurrentGame.Unload();
                }
            }
        }

        private void closeGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame != null) Game.CurrentGame.Unload();
        }

        private void debugBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            debugBar.Visible = !debugBar.Visible;
            this.Height += debugBar.Height * (debugBar.Visible ? 1 : -1);
            debugBarToolStripMenuItem.Checked = debugBar.Visible;
        }

        private void showHitboxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.DrawHitboxes = !Engine.Instance.DrawHitboxes;
            showHitboxesToolStripMenuItem.Checked = Engine.Instance.DrawHitboxes;
        }

        private void invincibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.Invincible = !Engine.Instance.Invincible;
            invincibilityToolStripMenuItem.Checked = Engine.Instance.Invincible;
        }

        private void gravityFlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Game.CurrentGame.GravityFlip = !Game.CurrentGame.GravityFlip;
            gravityFlipToolStripMenuItem.Checked = Game.CurrentGame.GravityFlip;
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Game.CurrentGame.Reset();
        }

        private void keyboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Keyboard keyform = new Keyboard();
            keyform.Show();
        }
    }
}
