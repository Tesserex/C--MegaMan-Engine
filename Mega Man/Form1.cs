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
        private string settingsPath;
        private CustomNtscForm customNtscForm = new CustomNtscForm();

        public Form1()
        {
            InitializeComponent();

            try
            {
                LoadConfig();
            }
            catch
            {
                MessageBox.Show("The config file could was not loaded successfully.");
            }

            ResizeScreen(Const.PixelsAcross, Const.PixelsDown);
            this.xnaImage.SetSize();

            Game.ScreenSizeChanged += new EventHandler<ScreenSizeChangedEventArgs>(Game_ScreenSizeChanged);
            Engine.Instance.GameLogicTick += new GameTickEventHandler(Instance_GameLogicTick);

            customNtscForm.Apply += new Action(customNtscForm_Apply);
        }

        protected override void OnClosed(EventArgs e)
        {
            // write settings to file
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
            this.xnaImage.SetSize();
        }

        private void ResizeScreen(int width, int height)
        {
            // tell the image not to get crushed by the form
            this.xnaImage.Dock = DockStyle.None;
            // tell the form to fit the image
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.xnaImage.Width = width;
            this.xnaImage.Height = height;
            // now remember the form size
            int tempheight = this.Height;
            int tempwidth = this.Width;
            // now un-autosize to re-enable resizing
            this.AutoSize = false;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            // reset the form size
            tempheight += debugBar.Height;
            this.Height = tempheight;
            this.Width = tempwidth;
            // redock the image
            this.xnaImage.Dock = DockStyle.Fill;
        }

        void Instance_GameLogicTick(GameTickEventArgs e)
        {
            float fps = 1 / e.TimeElapsed;
            fpsLabel.Text = "FPS: " + fps.ToString("N2");
            thinkLabel.Text = "Busy: " + (Engine.Instance.ThinkTime * 100).ToString("N0") + "%";
            entityLabel.Text = "Entities: " + GameEntity.ActiveCount.ToString();
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
                    // this builds a dialog message to tell the user where the error is in the XML file

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
            if (Game.CurrentGame == null) return;
            Game.CurrentGame.GravityFlip = !Game.CurrentGame.GravityFlip;
            gravityFlipToolStripMenuItem.Checked = Game.CurrentGame.GravityFlip;
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame != null) Game.CurrentGame.Reset();
        }

        private void keyboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Keyboard keyform = new Keyboard();
            keyform.Show();
        }

        private void sprites4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.SpritesFour = !Engine.Instance.SpritesFour;
            spries4ToolStripMenuItem.Checked = Engine.Instance.SpritesFour;
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.Background = !Engine.Instance.Background;
            backgroundToolStripMenuItem.Checked = Engine.Instance.Background;
        }

        private void sprites1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.SpritesOne = !Engine.Instance.SpritesOne;
            sprites1ToolStripMenuItem.Checked = Engine.Instance.SpritesOne;
        }

        private void sprites2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.SpritesTwo = !Engine.Instance.SpritesTwo;
            sprites2ToolStripMenuItem.Checked = Engine.Instance.SpritesTwo;
        }

        private void sprites3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.SpritesThree = !Engine.Instance.SpritesThree;
            sprites3ToolStripMenuItem.Checked = Engine.Instance.SpritesThree;
        }

        private void foregroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.Foreground = !Engine.Instance.Foreground;
            foregroundToolStripMenuItem.Checked = Engine.Instance.Foreground;
        }

        private void xnaImage_Click(object sender, EventArgs e)
        {

        }

        private void screen1XMenu_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame == null) ResizeScreen(Const.PixelsAcross, Const.PixelsDown);
            else ResizeScreen(Game.CurrentGame.PixelsAcross, Game.CurrentGame.PixelsDown);
            screen1XMenu.Checked = true;
            screen2XMenu.Checked = false;
            screenNTSCMenu.Checked = false;
            xnaImage.NTSC = false;
        }

        private void screen2XMenu_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame == null) ResizeScreen(Const.PixelsAcross * 2, Const.PixelsDown * 2);
            else ResizeScreen(Game.CurrentGame.PixelsAcross * 2, Game.CurrentGame.PixelsDown * 2);
            screen2XMenu.Checked = true;
            screen1XMenu.Checked = false;
            screenNTSCMenu.Checked = false;
            xnaImage.NTSC = false;
        }

        private void smoothedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.MagFilter = Microsoft.Xna.Framework.Graphics.TextureFilter.GaussianQuad;
            smoothedToolStripMenuItem.Checked = true;
            pixellatedToolStripMenuItem.Checked = false;
        }

        private void pixellatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.MagFilter = Microsoft.Xna.Framework.Graphics.TextureFilter.Point;
            smoothedToolStripMenuItem.Checked = false;
            pixellatedToolStripMenuItem.Checked = true;
        }

        private void screenNTSCMenu_Click(object sender, EventArgs e)
        {
            ResizeScreen(602, 448);
            screenNTSCMenu.Checked = true;
            screen2XMenu.Checked = false;
            screen1XMenu.Checked = false;
            xnaImage.NTSC = true;
        }

        private void ntscComposite_Click(object sender, EventArgs e)
        {
            screenNTSCMenu_Click(sender, e);
            ntscComposite.Checked = true;
            ntscSVideo.Checked = false;
            ntscRGB.Checked = false;
            ntscCustom.Checked = false;
            xnaImage.ntscInit(snes_ntsc_setup_t.snes_ntsc_composite);
        }

        private void ntscSVideo_Click(object sender, EventArgs e)
        {
            screenNTSCMenu_Click(sender, e);
            ntscSVideo.Checked = true;
            ntscRGB.Checked = false;
            ntscComposite.Checked = false;
            ntscCustom.Checked = false;
            xnaImage.ntscInit(snes_ntsc_setup_t.snes_ntsc_svideo);
        }

        private void ntscRGB_Click(object sender, EventArgs e)
        {
            screenNTSCMenu_Click(sender, e);
            ntscSVideo.Checked = false;
            ntscRGB.Checked = true;
            ntscComposite.Checked = false;
            ntscCustom.Checked = false;
            xnaImage.ntscInit(snes_ntsc_setup_t.snes_ntsc_rgb);
        }

        private void ntscCustom_Click(object sender, EventArgs e)
        {
            customNtscForm.Show();
        }

        private void customNtscForm_Apply()
        {
            screenNTSCMenu_Click(ntscCustom, new EventArgs());
            ntscCustom.Checked = true;
            ntscSVideo.Checked = false;
            ntscRGB.Checked = false;
            ntscComposite.Checked = false;
            xnaImage.ntscInit(new snes_ntsc_setup_t(customNtscForm.Hue, customNtscForm.Saturation, customNtscForm.Contrast, customNtscForm.Brightness,
                customNtscForm.Sharpness, customNtscForm.Gamma, customNtscForm.Resolution, customNtscForm.Artifacts, customNtscForm.Fringing, customNtscForm.Bleed, true));
        }
    }
}
