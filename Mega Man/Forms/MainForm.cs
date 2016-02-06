using System;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using MegaMan.IO.Xml;

namespace MegaMan.Engine
{
    public partial class MainForm : Form
    {
        private string settingsPath;
        private readonly CustomNtscForm customNtscForm = new CustomNtscForm();
        private int widthZoom, heightZoom, width, height;

        public MainForm()
        {
            InitializeComponent();

#if !DEBUG
            debugBar.Hide();
            debugBar.Height = 0;
            menuStrip1.Items.Remove(debugToolStripMenuItem);
#endif

            try
            {
                LoadConfig();
            }
            catch
            {
                MessageBox.Show("The config file could was not loaded successfully.");
            }

            widthZoom = heightZoom = 1;
            DefaultScreen();
            xnaImage.SetSize();

            Game.ScreenSizeChanged += Game_ScreenSizeChanged;
            Engine.Instance.GameLogicTick += Instance_GameLogicTick;

            Engine.Instance.OnException += Engine_Exception;

            customNtscForm.Apply += customNtscForm_Apply;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var path = args[1];
                var start = (args.Length > 2) ? args[2] : null;

                LoadGame(path, args.Skip(2).ToList());
            }
        }

        private void DefaultScreen()
        {
            width = Const.PixelsAcross;
            height = Const.PixelsDown;

            ResizeScreen();
        }

        protected override void OnDeactivate(EventArgs e)
        {
            Engine.Instance.Stop();
            base.OnDeactivate(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            Engine.Instance.Start();
            base.OnActivated(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            // write settings to file
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(settingsPath, null)
            {
                Indentation = 1,
                IndentChar = '\t',
                Formatting = System.Xml.Formatting.Indented
            };

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
                if (keys != null)
                {
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
        }

        void Game_ScreenSizeChanged(object sender, ScreenSizeChangedEventArgs e)
        {
            if (width != 256 || height != 224)
            {
                xnaImage.NTSC = false;
            }

            width = e.PixelsAcross;
            height = e.PixelsDown;
            // force resize so xnaImage is correct
            ResizeScreen(width, height);
            xnaImage.SetSize();

            if (xnaImage.NTSC)
            {
                ResizeScreen(602, 448);
            }
            else
            {
                // normal zoomed size
                ResizeScreen();
            }
        }

        private void ResizeScreen(int? newWidth = null, int? newHeight = null)
        {
            if (newWidth == null)
            {
                newWidth = width * widthZoom;
            }
            if (newHeight == null)
            {
                newHeight = height * heightZoom;
            }

            // tell the image not to get crushed by the form
            xnaImage.Dock = DockStyle.None;
            // tell the form to fit the image
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            xnaImage.Width = newWidth.Value;
            xnaImage.Height = newHeight.Value;
            // now remember the form size
            int tempheight = Height;
            int tempwidth = Width;
            // now un-autosize to re-enable resizing
            AutoSize = false;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            // reset the form size
            tempheight += debugBar.Height;
            Height = tempheight;
            Width = tempwidth;
            // redock the image
            xnaImage.Dock = DockStyle.Fill;
        }

        void Instance_GameLogicTick(GameTickEventArgs e)
        {
            float fps = 1 / e.TimeElapsed;
            fpsLabel.Text = "FPS: " + fps.ToString("N2");
            thinkLabel.Text = "Busy: " + (Engine.Instance.ThinkTime * 100).ToString("N0") + "%";
            entityLabel.Text = "Entities: " + Game.DebugEntitiesAlive();
            fpsCapLabel.Text = "FPS Cap: " + Engine.Instance.FPS;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                LoadGame(dialog.FileName);
            }
        }

        private void LoadGame(string path, List<string> pathArgs = null)
        {
            try
            {
                Game.Load(path, pathArgs);
                Text = Game.CurrentGame.Name;
            }
            catch (GameXmlException ex)
            {
                // this builds a dialog message to tell the user where the error is in the XML file

                StringBuilder message = new StringBuilder("There is an error in one of your game files.\n\n");
                if (ex.File != null) message.Append("File: ").Append(ex.File).Append('\n');
                if (ex.Line != 0) message.Append("Line: ").Append(ex.Line.ToString()).Append('\n');
                if (ex.Entity != null) message.Append("Entity: ").Append(ex.Entity).Append('\n');
                if (ex.Tag != null) message.Append("Tag: ").Append(ex.Tag).Append('\n');
                if (ex.Attribute != null) message.Append("Attribute: ").Append(ex.Attribute).Append('\n');

                message.Append("\n").Append(ex.Message);

                MessageBox.Show(message.ToString(), "Game Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Game.CurrentGame.Unload();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                MessageBox.Show("I'm sorry, I couldn't the following file. Perhaps the file path is incorrect?\n\n" + ex.Message, "C# MegaMan Engine", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Game.CurrentGame.Unload();
            }
            catch (XmlException ex)
            {
                MessageBox.Show("Your XML is badly formatted.\n\nFile: " + ex.SourceUri + "\n\nError: " + ex.Message, "C# MegaMan Engine", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Game.CurrentGame.Unload();
            }

            this.OnActivated(new EventArgs());
        }

        private void closeGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseGame();
        }

        private void CloseGame()
        {
            if (Game.CurrentGame != null)
            {
                Game.CurrentGame.Unload();
                this.xnaImage.Clear();
                Text = "Mega Man";
            }
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

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.ToggleLayerVisibility(0);
            backgroundToolStripMenuItem.Checked = Engine.Instance.GetLayerVisibility(0);
        }

        private void sprites1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.ToggleLayerVisibility(1);
            sprites1ToolStripMenuItem.Checked = Engine.Instance.GetLayerVisibility(1);
        }

        private void sprites2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.ToggleLayerVisibility(2);
            sprites2ToolStripMenuItem.Checked = Engine.Instance.GetLayerVisibility(2);
        }

        private void sprites3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.ToggleLayerVisibility(3);
            sprites3ToolStripMenuItem.Checked = Engine.Instance.GetLayerVisibility(3);
        }

        private void sprites4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.ToggleLayerVisibility(4);
            spries4ToolStripMenuItem.Checked = Engine.Instance.GetLayerVisibility(4);
        }

        private void foregroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.ToggleLayerVisibility(5);
            foregroundToolStripMenuItem.Checked = Engine.Instance.GetLayerVisibility(5);
        }

        private void screen1XMenu_Click(object sender, EventArgs e)
        {
            widthZoom = heightZoom = 1;
            ScreenSizeMultiple();
            screen1XMenu.Checked = true;
        }

        private void screen2XMenu_Click(object sender, EventArgs e)
        {
            widthZoom = heightZoom = 2;
            ScreenSizeMultiple();
            screen2XMenu.Checked = true;
        }

        private void screen3XMenu_Click(object sender, EventArgs e)
        {
            widthZoom = heightZoom = 3;
            ScreenSizeMultiple();
            screen3XMenu.Checked = true;
        }

        private void screen4XMenu_Click(object sender, EventArgs e)
        {
            widthZoom = heightZoom = 4;
            ScreenSizeMultiple();
            screen4XMenu.Checked = true;
        }

        private void ScreenSizeMultiple()
        {
            if (Game.CurrentGame == null)
            {
                DefaultScreen();
            }
            else
            {
                ResizeScreen();
            }

            screen1XMenu.Checked = false;
            screen2XMenu.Checked = false;
            screen3XMenu.Checked = false;
            screen4XMenu.Checked = false;
            screenNTSCMenu.Checked = false;
            xnaImage.NTSC = false;
        }

        private void smoothedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.FilterState = Microsoft.Xna.Framework.Graphics.SamplerState.LinearClamp;
            smoothedToolStripMenuItem.Checked = true;
            pixellatedToolStripMenuItem.Checked = false;
        }

        private void pixellatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.FilterState = Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp;
            smoothedToolStripMenuItem.Checked = false;
            pixellatedToolStripMenuItem.Checked = true;
        }

        private void screenNTSCMenu_Click(object sender, EventArgs e)
        {
            if (width != 256 || height != 224) return;

            widthZoom = heightZoom = 1;
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

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame != null) Game.CurrentGame.Unload();
            Application.Exit();
        }

#region Debug Menu

        private void debugBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            debugBar.Visible = !debugBar.Visible;
            Height += debugBar.Height * (debugBar.Visible ? 1 : -1);
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
            gravityFlipToolStripMenuItem.Checked = Game.CurrentGame.DebugFlipGravity();
        }

        private void framerateUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.FPS += 10;
        }

        private void framerateDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Engine.Instance.FPS > 10) Engine.Instance.FPS -= 10;
        }

        private void emptyHealthMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame != null)
            {
                Game.CurrentGame.DebugEmptyHealth();
            }
        }

        private void fillHealthMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame != null)
            {
                Game.CurrentGame.DebugFillHealth();
            }
        }

        private void emptyWeaponMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame != null)
            {
                Game.CurrentGame.DebugEmptyWeapon();
            }
        }

        private void fillWeaponMenuIem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame != null)
            {
                Game.CurrentGame.DebugFillWeapon();
            }
        }

#endregion

        private void musicMenuItem_Click(object sender, EventArgs e)
        {
            musicMenuItem.Checked = !musicMenuItem.Checked;
            Engine.Instance.SoundSystem.MusicEnabled = musicMenuItem.Checked;
        }

        private void sfxMenuItem_Click(object sender, EventArgs e)
        {
            sfxMenuItem.Checked = !sfxMenuItem.Checked;
            Engine.Instance.SoundSystem.SfxEnabled = sfxMenuItem.Checked;
        }

        private void sq1MenuItem_Click(object sender, EventArgs e)
        {
            sq1MenuItem.Checked = !sq1MenuItem.Checked;
            Engine.Instance.SoundSystem.SquareOne = sq1MenuItem.Checked;
        }

        private void sq2MenuItem_Click(object sender, EventArgs e)
        {
            sq2MenuItem.Checked = !sq2MenuItem.Checked;
            Engine.Instance.SoundSystem.SquareTwo = sq2MenuItem.Checked;
        }

        private void triMenuItem_Click(object sender, EventArgs e)
        {
            triMenuItem.Checked = !triMenuItem.Checked;
            Engine.Instance.SoundSystem.Triangle = triMenuItem.Checked;
        }

        private void noiseMenuItem_Click(object sender, EventArgs e)
        {
            noiseMenuItem.Checked = !noiseMenuItem.Checked;
            Engine.Instance.SoundSystem.Noise = noiseMenuItem.Checked;
        }

        private void screenshotMenuItem_Click(object sender, EventArgs e)
        {
            var capDir = Path.Combine(Application.StartupPath, "screenshots");
            if (!Directory.Exists(capDir)) System.IO.Directory.CreateDirectory(capDir);

            string capPath;
            int capNum = 1;

            do
            {
                capPath = Path.Combine(capDir, String.Format("{0}.png", capNum));
                capNum++;
            } while (File.Exists(capPath));

            using (var stream = File.OpenWrite(capPath))
            {
                xnaImage.SaveCap(stream);
            }
        }

        private void hideMenuItem_Click(object sender, EventArgs e)
        {
            if (menuStrip1.Visible)
            {
                Height -= menuStrip1.Height;
                menuStrip1.Visible = false;
            }
            else
            {
                menuStrip1.Visible = true;
                Height += menuStrip1.Height;
            }
        }

        private void Engine_Exception(Exception e)
        {
            MessageBox.Show(this, e.Message, "Game Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            CloseGame();
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.TopMost)
            {
                this.TopMost = false;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
                menuStrip1.Visible = !hideMenuItem.Checked;
#if DEBUG
                debugBar.Visible = true;
#endif
            }
            else
            {
                this.TopMost = true;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                menuStrip1.Visible = false;
#if DEBUG
                debugBar.Visible = false;
#endif
            }
        }
    }
}
