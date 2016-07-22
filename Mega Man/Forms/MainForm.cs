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

        ToolStripMenuItem previousScreenSizeSelection; // Remember previous screen selection to fullscreen option. Then when fullscreen is quitted, it goes back to this option
        private bool fullScreenToolStripMenuItem_IsMaximized;
        public static bool pauseEngine;

        #region Code used by windows messages
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_INITMENUPOPUP = 0x0117;
        private const int WM_UNINITMENUPOPUP = 0x0125;
        #endregion

        private bool menu, altKeyDown, gotFocus; // menu is either used when context menu or title bar menu is opened
                                                 // altKeyDown is exclusively used to know if it is the menu bar is activated by alt key

        #region Handle Engine pausing
        // Lots of functions used to determine what is happening and set engine activated/deactivated state

        /// <summary>
        /// Function which is called by events, checks conditions to know if engine is active/unactive
        /// </summary>
        private void HandleEngineActivation()
        {
            altKeyDown = false;

            if (menu || gotFocus == false || WindowState == FormWindowState.Minimized) Engine.Instance.Stop();
            else
            {
                Engine.Instance.Start();
                menu = false;   // If here, no more focus or Window is minimized, so menu is closed for surre
            }
        }

        /// <summary>
        /// Only event to be called when minimizing by clicking on tray icon.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            menu = false;
            HandleEngineActivation();
        }

        /// <summary>
        /// Called on focus lost.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);

            gotFocus = menu = false; // Menu is sure to be closed
            HandleEngineActivation();
        }

        /// <summary>
        /// Called on focus
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            gotFocus = true;
            HandleEngineActivation();
        }

        /// <summary>
        /// Called on moving form
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);

            menu = false; // Menu is sure to be closed.
            Engine.Instance.Stop();
        }

        /// <summary>
        /// Used because it is called when moving is stopped.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);

            menu = false;
            gotFocus = true;
            Engine.Instance.Start();
        }

        /// <summary>
        /// If user happens to find a way to have game deactivated when it shouldn't be, user will have reflex to click
        /// the game image. So when it is clicked, we must restard engine.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xnaImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.XButton1 && e.Button != MouseButtons.XButton2)
            {
                menu = false;
                gotFocus = true;
                Engine.Instance.Start();
            }
        }

        /// <summary>
        /// Menu was selected but is no more
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip1_MenuDeactivate(object sender, EventArgs e)
        {
            menu = false;
            HandleEngineActivation();
        }

        /// <summary>
        /// Menubar is activated. It can be by a mouse click (any), alt key, etc...
        /// To prevent some problems, it is only used by alt key press.
        /// To know if this key is pressed, we check it using ProcessCmdKey
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip1_MenuActivate(object sender, EventArgs e)
        {
            if (altKeyDown) menu = true;
            HandleEngineActivation();
        }

        /// <summary>
        /// Call by every menu clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                menu = true;
                HandleEngineActivation();
            }
        }

        /// <summary>
        /// If a menu is dropped down, make sure engine is stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            menu = true;
            HandleEngineActivation();

            try
            {
                gravityFlipToolStripMenuItem.Checked = Game.CurrentGame.GetFlipGravity();
            }
            catch (Exception)
            {
                gravityFlipToolStripMenuItem.Checked = false;
            }
        }

        /// <summary>
        /// Interrupt Windows messages. Used to know if context menu is opened.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_INITMENUPOPUP)
            {
                menu = true;
                HandleEngineActivation();
            }
            else if (m.Msg == WM_UNINITMENUPOPUP)
            {
                menu = false;
                HandleEngineActivation();
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Used to know if alt key is pressed.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((msg.Msg == WM_SYSKEYDOWN))
            {
                if (keyData == (Keys.Menu | Keys.Alt))
                {
                    altKeyDown = true;
                    if (fullScreenToolStripMenuItem.Checked)
                    {
                        menuStrip1.Visible = !menuStrip1.Visible;
                    }
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Pause engine or restart it depending on previous state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pauseEngineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pauseEngineToolStripMenuItem.Checked = !pauseEngineToolStripMenuItem.Checked;

            pauseEngine = pauseEngineToolStripMenuItem.Checked;

            if (pauseEngine) Engine.Instance.Stop();
            else
            {
                Engine.Instance.Start();
                if (Engine.Instance.SoundSystem.MusicEnabled != musicMenuItem.Checked)
                {
                    Engine.Instance.SoundSystem.MusicEnabled = musicMenuItem.Checked;
                }
            }
        }

        /// <summary>
        /// Unpause engine.
        /// </summary>
        /// <remarks>
        /// This function is slightly different from when engine is off from pauseEngineToolStripMenuItem_Click.
        /// When engine is off and pauseEngineToolStripMenuItem_Click is called, it also call Engine.Instance.Start. pauseOff doesn't.
        /// pauseOff is for a case Engine.Instance.Start() is gonna be called but by something else. It's not good to keep calling Engine.Instance.Start when it is already started.
        /// </remarks>
        private void pauseOff()
        {
            if (pauseEngineToolStripMenuItem.Checked == true)
            {
                pauseEngineToolStripMenuItem.Checked = pauseEngine = false;

                if (Engine.Instance.SoundSystem.MusicEnabled != musicMenuItem.Checked)
                {
                    Engine.Instance.SoundSystem.MusicEnabled = musicMenuItem.Checked;
                }
            }
        }
        #endregion

        public MainForm()
        {
            InitializeComponent();

            previousScreenSizeSelection = screen1XMenu;

            menu = gotFocus = altKeyDown = false;

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
            FormWindowState previousWindowState = WindowState;

            WindowState = FormWindowState.Normal;

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
            WindowState = previousWindowState;
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

        private void SetLayersVisibilityFromSettings()
        {
            Engine.Instance.SetLayerVisibility(0, backgroundToolStripMenuItem.Checked);
            Engine.Instance.SetLayerVisibility(1, sprites1ToolStripMenuItem.Checked);
            Engine.Instance.SetLayerVisibility(2, sprites2ToolStripMenuItem.Checked);
            Engine.Instance.SetLayerVisibility(3, sprites3ToolStripMenuItem.Checked);
            Engine.Instance.SetLayerVisibility(4, sprites4ToolStripMenuItem.Checked);
            Engine.Instance.SetLayerVisibility(5, foregroundToolStripMenuItem.Checked);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                pauseOff();

                LoadGame(dialog.FileName);
                SetLayersVisibilityFromSettings();
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
            if (Game.CurrentGame != null)
            {
                pauseOff();

                Game.CurrentGame.Reset();
                SetLayersVisibilityFromSettings();
            }
        }

        private void keyboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Keyboard keyform = new Keyboard();
            keyform.Show();
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.ToggleLayerVisibility(0);
            backgroundToolStripMenuItem.Checked = !backgroundToolStripMenuItem.Checked;
        }

        private void sprites1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.ToggleLayerVisibility(1);
            sprites1ToolStripMenuItem.Checked = !sprites1ToolStripMenuItem.Checked;
        }

        private void sprites2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.ToggleLayerVisibility(2);
            sprites2ToolStripMenuItem.Checked = !sprites2ToolStripMenuItem.Checked;
        }

        private void sprites3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.ToggleLayerVisibility(3);
            sprites3ToolStripMenuItem.Checked = !sprites3ToolStripMenuItem.Checked;
        }

        private void sprites4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.ToggleLayerVisibility(4);
            sprites4ToolStripMenuItem.Checked = !sprites4ToolStripMenuItem.Checked;
        }

        private void foregroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.ToggleLayerVisibility(5);
            foregroundToolStripMenuItem.Checked = !foregroundToolStripMenuItem.Checked;
        }

        private void activateAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!backgroundToolStripMenuItem.Checked) backgroundToolStripMenuItem_Click(sender, e);
            if (!sprites1ToolStripMenuItem.Checked) sprites1ToolStripMenuItem_Click(sender, e);
            if (!sprites2ToolStripMenuItem.Checked) sprites2ToolStripMenuItem_Click(sender, e);
            if (!sprites3ToolStripMenuItem.Checked) sprites3ToolStripMenuItem_Click(sender, e);
            if (!sprites4ToolStripMenuItem.Checked) sprites4ToolStripMenuItem_Click(sender, e);
            if (!foregroundToolStripMenuItem.Checked) foregroundToolStripMenuItem_Click(sender, e);
        }

        private void screen1XMenu_Click(object sender, EventArgs e)
        {
            previousScreenSizeSelection = screen1XMenu;
            widthZoom = heightZoom = 1;
            ScreenSizeMultiple();
            AllScreenResolutionOffBut(ref screen1XMenu);
        }

        private void screen2XMenu_Click(object sender, EventArgs e)
        {
            previousScreenSizeSelection = screen2XMenu;
            widthZoom = heightZoom = 2;
            ScreenSizeMultiple();
            AllScreenResolutionOffBut(ref screen2XMenu);
        }

        private void screen3XMenu_Click(object sender, EventArgs e)
        {
            previousScreenSizeSelection = screen3XMenu;
            widthZoom = heightZoom = 3;
            ScreenSizeMultiple();
            AllScreenResolutionOffBut(ref screen3XMenu);
        }

        private void screen4XMenu_Click(object sender, EventArgs e)
        {
            previousScreenSizeSelection = screen4XMenu;
            widthZoom = heightZoom = 4;
            ScreenSizeMultiple();
            AllScreenResolutionOffBut(ref screen4XMenu);
        }

        private void ScreenSizeMultiple()
        {
            WindowState = FormWindowState.Normal;
            if (Game.CurrentGame == null)
            {
                DefaultScreen();
            }
            else
            {
                ResizeScreen();
            }

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
            previousScreenSizeSelection = screenNTSCMenu;

            if (width != 256 || height != 224) return;

            widthZoom = heightZoom = 1;
            ResizeScreen(602, 448);
            
            AllScreenResolutionOffBut(ref screenNTSCMenu);
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
        
        private void defaultFramerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.FPS = 60;
        }

        private void emptyHealthMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame != null)
            {
                if (Engine.Instance.Invincible)
                {
                    Engine.Instance.Invincible = false;
                    Game.CurrentGame.DebugEmptyHealth();
                    Engine.Instance.Invincible = true;
                }
                else Game.CurrentGame.DebugEmptyHealth();
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
            if (!pauseEngine) Engine.Instance.SoundSystem.MusicEnabled = musicMenuItem.Checked;
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

        private void AllScreenResolutionOffBut(ref ToolStripMenuItem itemToKeepChecked)
        {
            fullScreenToolStripMenuItem.Checked = screenNTSCMenu.Checked = false;
            screen1XMenu.Checked = screen2XMenu.Checked = screen3XMenu.Checked = screen4XMenu.Checked = false;

            itemToKeepChecked.Checked = true;
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fullScreenToolStripMenuItem.Checked = !fullScreenToolStripMenuItem.Checked;

            if (fullScreenToolStripMenuItem.Checked)
            {
                fullScreenToolStripMenuItem_IsMaximized = (this.WindowState == FormWindowState.Maximized) ?  true : false;

                AllScreenResolutionOffBut(ref fullScreenToolStripMenuItem);
                xnaImage.NTSC = false;
                this.TopMost = true;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                menuStrip1.Visible = false;
#if DEBUG
                debugBar.Visible = false;
#endif
            }
            else
            {
                AllScreenResolutionOffBut(ref previousScreenSizeSelection);
                this.TopMost = false;
                this.FormBorderStyle = FormBorderStyle.Sizable;

                if (fullScreenToolStripMenuItem_IsMaximized) this.WindowState = FormWindowState.Maximized;
                else this.WindowState = FormWindowState.Normal;

                menuStrip1.Visible = !hideMenuItem.Checked;
#if DEBUG
                debugBar.Visible = true;
#endif

                // NTSC has special specifications
                if (previousScreenSizeSelection.Name == screenNTSCMenu.Name)
                {
                    screenNTSCMenu_Click(sender, e);
                }
            }
        }
    }
}
