using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using MegaMan.IO.Xml;
using MegaMan.Engine.Forms;

namespace MegaMan.Engine
{
    public partial class MainForm : Form
    {
        public class NTSC_CustomParameters
        {
            public int hue;
            public int saturation;
            public int contrast;
            public int brightness;
            public int sharpness;
            public int gamma;
            public int resolution;
            public int artifacts;
            public int fringing;
            public int bleed;

            public NTSC_CustomParameters()
            {
                hue = saturation = contrast = brightness = sharpness = gamma = resolution = artifacts = fringing = bleed = 0;
            }
        }



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

            if (!screenToolStripMenuItem.Pressed)
            {
                menu = false;
                HandleEngineActivation();
            }
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

            customNtscForm.Apply += customNtscForm_ApplyFromForm;
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
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            if (WindowState == FormWindowState.Minimized)
            {
                Engine.Instance.Stop();
            }
            else
            {
                Engine.Instance.Start();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            var serializer = new XmlSerializer(typeof(UserSettings));
            var settings = new UserSettings() {
                Keys = new UserKeys() {
                    Up = GameInputKeys.Up,
                    Down = GameInputKeys.Down,
                    Left = GameInputKeys.Left,
                    Right = GameInputKeys.Right,
                    Jump = GameInputKeys.Jump,
                    Shoot = GameInputKeys.Shoot,
                    Start = GameInputKeys.Start,
                    Select = GameInputKeys.Select
                }
            };
            
            XmlTextWriter writer = new XmlTextWriter(settingsPath, null)
            {
                Indentation = 1,
                IndentChar = '\t',
                Formatting = Formatting.Indented
            };

            serializer.Serialize(writer, settings);

            writer.Close();
            base.OnClosed(e);
        }

        private void LoadConfig()
        {
            settingsPath = Path.Combine(Application.StartupPath, "settings.xml");
            if (File.Exists(settingsPath))
            {
                var serializer = new XmlSerializer(typeof(UserSettings));
                using (var file = File.Open(settingsPath, FileMode.Open))
                {
                    var settings = (UserSettings)serializer.Deserialize(file);
                    GameInputKeys.Up = settings.Keys.Up;
                    GameInputKeys.Down = settings.Keys.Down;
                    GameInputKeys.Left = settings.Keys.Left;
                    GameInputKeys.Right = settings.Keys.Right;
                    GameInputKeys.Jump = settings.Keys.Jump;
                    GameInputKeys.Shoot = settings.Keys.Shoot;
                    GameInputKeys.Start = settings.Keys.Start;
                    GameInputKeys.Select = settings.Keys.Select;
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
            
            SetXnaSize(width, height);

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
            if (fullScreenToolStripMenuItem.Checked)
            {
                menuStrip1.Visible = false;
            }

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
            if (debugBar.Visible)
            {
                tempheight += debugBar.Height;
            }

            // for some reason menu height is always still shown when the image is undocked
            if (!menuStrip1.Visible)
            {
                tempheight -= menuStrip1.Height;
            }

            Height = tempheight;
            Width = tempwidth;
            // redock the image
            xnaImage.Dock = DockStyle.Fill;
        }

        private void SetXnaSize(int width, int height)
        {
            xnaImage.Dock = DockStyle.None;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            xnaImage.Width = width;
            xnaImage.Height = height;
            xnaImage.SetSize();

            AutoSize = false;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
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
            catch (Exception ex)
            {
                MessageBox.Show("There was an error loading the game.\n\n" + ex.Message, "C# MegaMan Engine", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void toggleLayerVisibility(ToolStripMenuItem itemToToggleCheck, int layerIndex)
        {
            Engine.Instance.ToggleLayerVisibility(layerIndex);
            itemToToggleCheck.Checked = !itemToToggleCheck.Checked;
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleLayerVisibility(backgroundToolStripMenuItem, (int)UserSettingsEnums.Layers.Background);
        }

        private void sprites1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleLayerVisibility(sprites1ToolStripMenuItem, (int)UserSettingsEnums.Layers.Sprite1);
        }

        private void sprites2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleLayerVisibility(sprites2ToolStripMenuItem, (int)UserSettingsEnums.Layers.Sprite2);
        }

        private void sprites3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleLayerVisibility(sprites3ToolStripMenuItem, (int)UserSettingsEnums.Layers.Sprite3);
        }

        private void sprites4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleLayerVisibility(sprites4ToolStripMenuItem, (int)UserSettingsEnums.Layers.Sprite4);
        }

        private void foregroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleLayerVisibility(foregroundToolStripMenuItem, (int)UserSettingsEnums.Layers.Foreground);
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

        /// <summary>
        /// For screen option X1, X2, X3, X4, they use mostly similar code.
        /// </summary>
        /// <param name="index"></param>
        private void screenXMenuSelected(int index)
        {
            if (index == (int) UserSettingsEnums.Screen.X1) 
            {
                previousScreenSizeSelection = screen1XMenu;
                widthZoom = heightZoom = 1;
            }
            if (index == (int)UserSettingsEnums.Screen.X2)
            {
                previousScreenSizeSelection = screen2XMenu;
                widthZoom = heightZoom = 2;
            }
            if (index == (int)UserSettingsEnums.Screen.X3)
            {
                previousScreenSizeSelection = screen3XMenu;
                widthZoom = heightZoom = 3;
            }
            if (index == (int)UserSettingsEnums.Screen.X4)
            {
                previousScreenSizeSelection = screen4XMenu;
                widthZoom = heightZoom = 4;
            }
            ScreenSizeMultiple();
            AllScreenResolutionOffBut(previousScreenSizeSelection);
        }

        private void screen1XMenu_Click(object sender, EventArgs e)
        {
            screenXMenuSelected((int)UserSettingsEnums.Screen.X1);
        }

        private void screen2XMenu_Click(object sender, EventArgs e)
        {
            screenXMenuSelected((int)UserSettingsEnums.Screen.X2);
        }

        private void screen3XMenu_Click(object sender, EventArgs e)
        {
            screenXMenuSelected((int)UserSettingsEnums.Screen.X3);
        }

        private void screen4XMenu_Click(object sender, EventArgs e)
        {
            screenXMenuSelected((int)UserSettingsEnums.Screen.X4);
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

        private void screenNTSCSelected()
        {
            previousScreenSizeSelection = screenNTSCMenu;

            if (width != 256 || height != 224) return;

            widthZoom = heightZoom = 1;
            ResizeScreen(602, 448);

            AllScreenResolutionOffBut(screenNTSCMenu);
            xnaImage.NTSC = true;
        }

        private void screenNTSCMenu_Click(object sender, EventArgs e)
        {
            screenNTSCSelected();
        }

        private void ntscOptionClick(ToolStripMenuItem menuClicked, snes_ntsc_setup_t snes_ntsc_type)
        {
            screenNTSCSelected();
            NTSC_OptionsOffBut(menuClicked);
            xnaImage.ntscInit(snes_ntsc_type);
        }

        private void ntscComposite_Click(object sender, EventArgs e)
        {
            ntscOptionClick(ntscComposite, snes_ntsc_setup_t.snes_ntsc_composite);
        }

        private void ntscSVideo_Click(object sender, EventArgs e)
        {
            ntscOptionClick(ntscSVideo, snes_ntsc_setup_t.snes_ntsc_svideo);
        }

        private void ntscRGB_Click(object sender, EventArgs e)
        {
            ntscOptionClick(ntscRGB, snes_ntsc_setup_t.snes_ntsc_rgb);
        }

        private void ntscCustom_Click(object sender, EventArgs e)
        {
            customNtscForm.Show();
        }

        /// <summary>
        /// Receive all the values in a wrapper class
        /// </summary>
        /// <param name="values"></param>
        /// <remarks>Used to set values from config file read.</remarks>
        private void customNtscForm_ApplyFromWrapperObject(NTSC_CustomParameters wrapper)
        {
            customNtscForm_Apply(new snes_ntsc_setup_t(wrapper.hue, wrapper.saturation, wrapper.contrast, wrapper.brightness,
                wrapper.sharpness, wrapper.gamma, wrapper.resolution, wrapper.artifacts, wrapper.fringing, wrapper.bleed, true));
        }

        /// <summary>
        /// Build a snes_ntsc_setup_t item from form parameters
        /// </summary>
        private void customNtscForm_ApplyFromForm()
        {
            customNtscForm_Apply(new snes_ntsc_setup_t(customNtscForm.Hue, customNtscForm.Saturation, customNtscForm.Contrast, customNtscForm.Brightness,
                customNtscForm.Sharpness, customNtscForm.Gamma, customNtscForm.Resolution, customNtscForm.Artifacts, customNtscForm.Fringing, customNtscForm.Bleed, true));
        }

        private void customNtscForm_Apply(snes_ntsc_setup_t snes_ntsc_setup)
        {
            ntscOptionClick(ntscCustom, snes_ntsc_setup);
        }

        private void pixellatedVsSmoothedAllOffBut(ToolStripMenuItem itemToKeepChecked)
        {
            smoothedToolStripMenuItem.Checked = pixellatedToolStripMenuItem.Checked = false;

            itemToKeepChecked.Checked = true;
        }

        /// <summary>
        /// Function that execute code for Smoothed/Pixellated selection
        /// </summary>
        /// <param name="itemToKeepChecked"></param>
        /// <param name="samplerState"></param>
        private void pixellatedVsSmoothedCode(ToolStripMenuItem itemToKeepChecked, Microsoft.Xna.Framework.Graphics.SamplerState samplerState)
        {
            Engine.Instance.FilterState = samplerState;
            pixellatedVsSmoothedAllOffBut(itemToKeepChecked);
        }

        private void smoothedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pixellatedVsSmoothedCode(smoothedToolStripMenuItem, Microsoft.Xna.Framework.Graphics.SamplerState.LinearClamp);
        }

        private void pixellatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pixellatedVsSmoothedCode(pixellatedToolStripMenuItem, Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame != null) Game.CurrentGame.Unload();
            Application.Exit();
        }

        #region Debug Menu

        private void toggleDebugBar()
        {
            debugBar.Visible = !debugBar.Visible;
            Height += debugBar.Height * (debugBar.Visible ? 1 : -1);
            debugBarToolStripMenuItem.Checked = debugBar.Visible;
        }

        private void debugBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleDebugBar();
        }

        private void toggleShowHitBoxes()
        {
            Engine.Instance.DrawHitboxes = !Engine.Instance.DrawHitboxes;
            showHitboxesToolStripMenuItem.Checked = Engine.Instance.DrawHitboxes;
        }

        private void showHitboxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleShowHitBoxes();
        }

        private void toggleInvincibility()
        {
            Engine.Instance.Invincible = !Engine.Instance.Invincible;
            invincibilityToolStripMenuItem.Checked = Engine.Instance.Invincible;
        }

        private void invincibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleInvincibility();
        }

        private void gravityFlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame == null) return;
            gravityFlipToolStripMenuItem.Checked = Game.CurrentGame.DebugFlipGravity();
        }

        private void framerateUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Engine.Instance.FPS <= 490)
                Engine.Instance.FPS += 10;

            fpsCapLabel.Text = "FPS Cap: " + Engine.Instance.FPS;
        }

        private void framerateDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Engine.Instance.FPS > 10)
                Engine.Instance.FPS -= 10;

            fpsCapLabel.Text = "FPS Cap: " + Engine.Instance.FPS;
        }
        
        private void defaultFramerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Engine.Instance.FPS = 60;
            fpsCapLabel.Text = "FPS Cap: " + Engine.Instance.FPS;
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

        private void toggleMusic()
        {
            musicMenuItem.Checked = !musicMenuItem.Checked;
            if (!pauseEngine) Engine.Instance.SoundSystem.MusicEnabled = musicMenuItem.Checked;
        }

        private void musicMenuItem_Click(object sender, EventArgs e)
        {
            toggleMusic();
        }

        private void togglerSFX()
        {
            sfxMenuItem.Checked = !sfxMenuItem.Checked;
            Engine.Instance.SoundSystem.SfxEnabled = sfxMenuItem.Checked;
        }

        private void sfxMenuItem_Click(object sender, EventArgs e)
        {
            togglerSFX();
        }

        private void toggleSq1()
        {
            sq1MenuItem.Checked = !sq1MenuItem.Checked;
            Engine.Instance.SoundSystem.SquareOne = sq1MenuItem.Checked;
        }

        private void sq1MenuItem_Click(object sender, EventArgs e)
        {
            toggleSq1();
        }

        private void toggleSq2()
        {
            sq2MenuItem.Checked = !sq2MenuItem.Checked;
            Engine.Instance.SoundSystem.SquareTwo = sq2MenuItem.Checked;
        }

        private void sq2MenuItem_Click(object sender, EventArgs e)
        {
            toggleSq2();
        }

        private void toggleTri()
        {
            triMenuItem.Checked = !triMenuItem.Checked;
            Engine.Instance.SoundSystem.Triangle = triMenuItem.Checked;
        }

        private void triMenuItem_Click(object sender, EventArgs e)
        {
            toggleTri();
        }

        private void toggleNoise()
        {
            noiseMenuItem.Checked = !noiseMenuItem.Checked;
            Engine.Instance.SoundSystem.Noise = noiseMenuItem.Checked;
        }

        private void noiseMenuItem_Click(object sender, EventArgs e)
        {
            toggleNoise();
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

        private void hideMenu()
        {
            if (menuStrip1.Visible)
            {
                hideMenuItem.Checked = true;
                Height -= menuStrip1.Height;
                menuStrip1.Visible = false;
            }
            else
            {
                hideMenuItem.Checked = false;
                menuStrip1.Visible = true;
                Height += menuStrip1.Height;
            }
        }

        private void hideMenuItem_Click(object sender, EventArgs e)
        {
            hideMenu();
        }

        private void Engine_Exception(Exception e)
        {
            MessageBox.Show(this, e.Message, "Game Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            CloseGame();
        }

        private void AllScreenResolutionOffBut(ToolStripMenuItem itemToKeepChecked)
        {
            fullScreenToolStripMenuItem.Checked = screenNTSCMenu.Checked = false;
            screen1XMenu.Checked = screen2XMenu.Checked = screen3XMenu.Checked = screen4XMenu.Checked = false;

            itemToKeepChecked.Checked = true;
        }

        private void NTSC_OptionsOffBut(ToolStripMenuItem itemToKeepChecked)
        {
            ntscComposite.Checked = ntscSVideo.Checked = ntscRGB.Checked = ntscCustom.Checked = false;

            itemToKeepChecked.Checked = true;
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fullScreenToolStripMenuItem.Checked = !fullScreenToolStripMenuItem.Checked;

            if (fullScreenToolStripMenuItem.Checked)
            {
                fullScreenToolStripMenuItem_IsMaximized = (this.WindowState == FormWindowState.Maximized) ?  true : false;

                AllScreenResolutionOffBut(fullScreenToolStripMenuItem);
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
                AllScreenResolutionOffBut(previousScreenSizeSelection);
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
