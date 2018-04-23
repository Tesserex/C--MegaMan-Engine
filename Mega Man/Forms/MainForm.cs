using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices; // To use DllImport
using System.Text;
using System.Windows.Forms;
using System.Xml;
using MegaMan.Engine.Forms;
using MegaMan.Engine.Forms.MenuControllers;
using MegaMan.Engine.Forms.Settings;
using MegaMan.Engine.Input;
using MegaMan.IO.Xml;

namespace MegaMan.Engine
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_INITMENUPOPUP = 0x0117;
        private const int WM_UNINITMENUPOPUP = 0x0125;

        private List<IMenuController> controllers;
        private readonly SettingsService settingsService;

        private string lastGameWithPath, initialFolder;
        private int widthZoom, heightZoom, width, height;
        private bool wasMaximizedBeforeFullscreen;
        
        private bool menu, altKeyDown, gotFocus; // menu is either used when context menu or title bar menu is opened

        private readonly CustomNtscForm customNtscForm = new CustomNtscForm();
        private readonly Keyboard keyform = new Keyboard();
        private readonly LoadConfig loadConfigForm = new LoadConfig();
        private readonly DeleteConfigs deleteConfigsForm = new DeleteConfigs();

        private string CurrentGamePath
        {
            get { return Game.CurrentGame != null ? Game.CurrentGame.BasePath : string.Empty; }
        }

        private string CurrentGameTitle
        {
            get { return Game.CurrentGame != null ? Game.CurrentGame.Name : string.Empty; }
        }

        #region Handle Engine pausing
        // Lots of functions used to determine what is happening and set engine activated/deactivated state

        /// <summary>
        /// Function which is called by events, checks conditions to know if engine is active/unactive
        /// </summary>
        private void HandleEngineActivation()
        {
            altKeyDown = false;

            if (menu || gotFocus == false || WindowState == FormWindowState.Minimized)
                Engine.Instance.Stop();
            else
            {
                if (GetForegroundWindow() == this.Handle)
                {
                    Engine.Instance.Start();
                }
            }
        }

        /// <summary>
        /// A function is made because the event needs to be "triggered" at other places
        /// </summary>
        private void OnResizeCode()
        {
            if (!screenToolStripMenuItem.Pressed)
            {
                menu = false;
                HandleEngineActivation();
            }

            if (WindowState == FormWindowState.Maximized)
                xnaImage.NTSC = false;
        }

        /// <summary>
        /// Only event to be called when minimizing by clicking on tray icon.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            OnResizeCode();
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

#if DEBUG
            try
            {
                gravityFlipToolStripMenuItem.Checked = Game.CurrentGame.GetFlipGravity();
            }
            catch (Exception)
            {
                gravityFlipToolStripMenuItem.Checked = false;
            }
#endif
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
        /// Unpause engine.
        /// </summary>
        /// <remarks>
        /// This function is slightly different from when engine is off from pauseEngineToolStripMenuItem_Click.
        /// When engine is off and pauseEngineToolStripMenuItem_Click is called, it also call Engine.Instance.Start. pauseOff doesn't.
        /// pauseOff is for a case Engine.Instance.Start() is gonna be called but by something else. It's not good to keep calling Engine.Instance.Start when it is already started.
        /// </remarks>
        private void pauseOff()
        {
            pauseEngineToolStripMenuItem.Checked = false;
        }
        #endregion

        #region Form Events Openings/Closings
        public MainForm()
        {
            InitializeComponent();

            this.settingsService = new SettingsService();
            InitializeControllers();

            menu = gotFocus = altKeyDown = false;
            defaultConfigToolStripMenuItem.Checked = true;
            initialFolder = "";
            lastGameWithPath = null;

#if DEBUG
            
            this.gravityFlipToolStripMenuItem.Click += new System.EventHandler(this.gravityFlipToolStripMenuItem_Click);
            this.emptyHealthMenuItem.Click += new System.EventHandler(this.emptyHealthMenuItem_Click);
            this.fillHealthMenuItem.Click += new System.EventHandler(this.fillHealthMenuItem_Click);
            this.emptyWeaponMenuItem.Click += new System.EventHandler(this.emptyWeaponMenuItem_Click);
            this.fillWeaponMenuIem.Click += new System.EventHandler(this.fillWeaponMenuIem_Click);
#else
            debugBar.Hide();
            debugBar.Height = 0;
            menuStrip1.Items.Remove(debugToolStripMenuItem);
#endif

            widthZoom = heightZoom = 1;
            DefaultScreen();
            xnaImage.SetSize();

            Game.ScreenSizeChanged += Game_ScreenSizeChanged;
            Engine.Instance.GameLogicTick += Instance_GameLogicTick;

            Engine.Instance.OnException += Engine_Exception;

            customNtscForm.Apply += customNtscForm_ApplyFromForm;
            loadConfigForm.Apply += loadConfigSelectedInLoadConfigForm;
            keyform.FormClosed += (s, e) => AutosaveConfig(null);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Hide();
            customNtscForm.StartPosition = FormStartPosition.Manual;
            keyform.StartPosition = FormStartPosition.Manual;
            loadConfigForm.StartPosition = FormStartPosition.Manual;
            deleteConfigsForm.StartPosition = FormStartPosition.Manual;

            try
            {
                var args = Environment.GetCommandLineArgs();

                base.OnLoad(e);

                if (args.Length > 1)
                {
                    var path = args[1];
                    var start = (args.Length > 2) ? args[2] : null;

                    LoadGame(path, args.Skip(2).ToList());
                }
                else
                {
                    try
                    {
                        LoadGlobalConfigValues();
                        LoadCurrentConfig();

                        string autoLoadGame = settingsService.GetAutoLoadGame();
                        if (autoLoadGame != null)
                        {
                            if (!LoadGame(autoLoadGame, null, true))
                            {
                                // Game we try to autoload failed. Now set autoload to when no game is loaded
                                autoloadToolStripMenuItem.Checked = true;
                                SaveGlobalConfigValues();
                            }
                        }
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.Message); // If a line in config file is wrong, this is gonna tell user.
                        MessageBox.Show("The config file could was not loaded successfully.");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void InitializeControllers()
        {
            var b = new LayerVisibilityMenuController(backgroundToolStripMenuItem, Layers.Background);
            var s1 = new LayerVisibilityMenuController(sprites1ToolStripMenuItem, Layers.Sprite1);
            var s2 = new LayerVisibilityMenuController(sprites2ToolStripMenuItem, Layers.Sprite2);
            var s3 = new LayerVisibilityMenuController(sprites3ToolStripMenuItem, Layers.Sprite3);
            var s4 = new LayerVisibilityMenuController(sprites4ToolStripMenuItem, Layers.Sprite4);
            var f = new LayerVisibilityMenuController(foregroundToolStripMenuItem, Layers.Foreground);

            var scaleController = new ScreenScaleController();
            var screen1xControl = new ScreenSizeMenuController(scaleController, screen1XMenu, ScreenScale.X1);
            var screen2xControl = new ScreenSizeMenuController(scaleController, screen2XMenu, ScreenScale.X2);
            var screen3xControl = new ScreenSizeMenuController(scaleController, screen3XMenu, ScreenScale.X3);
            var screen4xControl = new ScreenSizeMenuController(scaleController, screen4XMenu, ScreenScale.X4);
            var fullScreenControl = new FullScreenMenuController(scaleController, fullScreenToolStripMenuItem);

            var compositeControl = new NtscMenuController(scaleController, ntscComposite, NTSC_Options.Composite);
            var svideoControl = new NtscMenuController(scaleController, ntscSVideo, NTSC_Options.S_Video);
            var rgbControl = new NtscMenuController(scaleController, ntscRGB, NTSC_Options.RGB);

            scaleController.SizeChanged += ScaleChanged;
            scaleController.NtscSet += ScaleController_NtscSet;

            var pixellatedCtrl = new ExclusiveController<PixellatedOrSmoothed>();

            this.controllers = new List<IMenuController>() {
                b, s1, s2, s3, s4, f,
                new ActivateAllMenuController(activateAllToolStripMenuItem, b, s1, s2, s3, s4, f),
                new AudioMenuController(sq1MenuItem, 1),
                new AudioMenuController(sq2MenuItem, 2),
                new AudioMenuController(triMenuItem, 3),
                new AudioMenuController(noiseMenuItem, 4),
                screen1xControl,
                screen2xControl,
                screen3xControl,
                screen4xControl,
                compositeControl,
                svideoControl,
                rgbControl,
                fullScreenControl,
                new PixellatedMenuController(pixellatedToolStripMenuItem, PixellatedOrSmoothed.Pixellated, pixellatedCtrl),
                new PixellatedMenuController(smoothedToolStripMenuItem, PixellatedOrSmoothed.Smoothed, pixellatedCtrl)
            };
        }

        private void ScaleController_NtscSet(object sender, ScreenScaleNtscEventArgs e)
        {
            NtscScreen(e.Setup);
        }

        private void ScaleChanged(object sender, ScreenScaleChangedEventArgs e)
        {
            ScaleScreen(e.Scale);
        }

        /// <summary>
        /// Things to do when closing engine, no matter the way
        /// </summary>
        public void close()
        {
            AutosaveConfig();
            if (Game.CurrentGame != null) Game.CurrentGame.Unload();
        }

        protected override void OnClosed(EventArgs e)
        {
            close();
            base.OnClosed(e);
        }
        #endregion

        #region Menus
        #region File Menu
        #region First section
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result;

            // If directory from xml is still valid, use it, else restore default one.
            if (Directory.Exists(initialFolder)) dialog.InitialDirectory = initialFolder;
            else dialog.InitialDirectory = Directory.GetCurrentDirectory();

            result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                initialFolder = Path.GetDirectoryName(dialog.FileName);

                AutosaveConfig();
                SaveGlobalConfigValues();

                pauseOff();

                LoadGame(dialog.FileName);
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame != null)
            {
                pauseOff();

                Game.CurrentGame.Reset();
                OnGameLoaded();
            }
        }
        
        private void CloseGame()
        {
            if (Game.CurrentGame != null)
            {
                AutosaveConfig();
                lastGameWithPath = null;
                LoadCurrentConfig();

                Game.CurrentGame.Unload();
                this.xnaImage.Clear();
                Text = "Mega Man";

                OnGameLoadedChanged();
            }
        }

        private void closeGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseGame();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            close();
            Application.Exit();
        }
        #endregion

        #region Second section
        /// <summary>
        /// Pause engine or restart it depending on previous state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pauseEngineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pauseEngineToolStripMenuItem.Checked = !pauseEngineToolStripMenuItem.Checked;

            if (pauseEngineToolStripMenuItem.Checked)
                Engine.Instance.Pause();
            else
                Engine.Instance.Unpause();
        }
        #endregion

        #region Third Section
        /// <summary>
        /// If true, on events where saving happens, no save (except if menu to save is picked).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autosaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autosaveToolStripMenuItem.Checked = !autosaveToolStripMenuItem.Checked;
            SaveGlobalConfigValues();
        }

        /// <summary>
        /// If defaultConfigToolStripMenuItem unchecked, uses a config specific to a game name in xml file for configs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void defaultConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutosaveConfig();
            defaultConfigToolStripMenuItem.Checked = !defaultConfigToolStripMenuItem.Checked;
            SaveGlobalConfigValues();
            LoadCurrentConfig();

            SetLayersVisibilityFromSettings();
        }

        private void LoadCurrentConfig()
        {
            if (defaultConfigToolStripMenuItem.Checked)
                LoadConfigFromSetting(this.settingsService.GetConfigForGame(""));
            else
                LoadConfigFromSetting(this.settingsService.GetConfigForGame(CurrentGamePath));
        }

        private void loadConfigSelectedInLoadConfigForm()
        {
            LoadConfigFromSetting(loadConfigForm.settingsSelected);
        }

        private void loadConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var userSettingsToPass = this.settingsService.GetSettings();

            loadConfigForm.Location = new Point(this.Location.X + (this.Size.Width - loadConfigForm.Size.Width) / 2, this.Location.Y + (this.Size.Height - loadConfigForm.Size.Height) / 2);
            loadConfigForm.TopMost = this.TopMost;
            loadConfigForm.showFormIfNeeded(CurrentGamePath, userSettingsToPass, defaultConfigToolStripMenuItem.Checked);
            loadConfigForm.TopMost = false;
        }
        private void saveConfigurationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void deleteConfigurationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var userSettingsToPass = this.settingsService.GetSettings();

            deleteConfigsForm.Location = new Point(this.Location.X + (this.Size.Width - deleteConfigsForm.Size.Width) / 2, this.Location.Y + (this.Size.Height - deleteConfigsForm.Size.Height) / 2);
            deleteConfigsForm.TopMost = this.TopMost;
            deleteConfigsForm.PrepareFormAndShowIfNeeded(userSettingsToPass, this.settingsService.SettingsFilePath, null);
            deleteConfigsForm.TopMost = false;
        }
        #endregion
        
        /// <summary>
        /// True for a game it autoloads when application starts (if none, this option is checked when no game is loaded)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentGamePath == "") autoloadToolStripMenuItem.Checked = true;
            else
            {
                autoloadToolStripMenuItem.Checked = !autoloadToolStripMenuItem.Checked;
            }

            SaveGlobalConfigValues();
        }
        #endregion
        
        private void keyboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            keyform.Location = new Point(this.Location.X + (this.Size.Width - keyform.Size.Width) / 2, this.Location.Y + (this.Size.Height - keyform.Size.Height) / 2);
            keyform.TopMost = this.TopMost;
            keyform.ShowDialog();
            keyform.TopMost = false;
            if (autosaveToolStripMenuItem.Checked) SaveConfig();
        }

        #region Screen Menu
        #region First Section

        public void ScaleScreen(ScreenScale scale)
        {
            if (scale == ScreenScale.Fullscreen)
                SetFullscreen();
            else
                UnsetFullscreen();

            if (scale == ScreenScale.X1)
            {
                widthZoom = heightZoom = 1;
            }
            else if (scale == ScreenScale.X2)
            {
                widthZoom = heightZoom = 2;
            }
            else if (scale == ScreenScale.X3)
            {
                widthZoom = heightZoom = 3;
            }
            else if (scale == ScreenScale.X4)
            {
                widthZoom = heightZoom = 4;
            }

            if (Game.CurrentGame == null)
            {
                DefaultScreen();
                xnaImage.NTSC = false;
            }
            else if (scale == ScreenScale.NTSC)
            {
                NtscScreen();
            }
            else
            {
                ResizeScreen();
                xnaImage.NTSC = false;
            }            
        }

        private void NtscScreen(snes_ntsc_setup_t setup = null)
        {
            if (width != 256 || height != 224) return;

            UnsetFullscreen();
            this.WindowState = FormWindowState.Normal;

            widthZoom = heightZoom = 1;
            ResizeScreen(602, 448);
            
            if (setup != null)            
                xnaImage.ntscInit(setup);

            xnaImage.NTSC = true;
        }

        private void UnsetFullscreen()
        {
            this.TopMost = false;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            if (wasMaximizedBeforeFullscreen) this.WindowState = FormWindowState.Maximized;
            else this.WindowState = FormWindowState.Normal;

            menuStrip1.Visible = !hideMenuItem.Checked;
#if DEBUG
            debugBar.Visible = true;
#endif
        }

        private void SetFullscreen()
        {
            wasMaximizedBeforeFullscreen = (this.WindowState == FormWindowState.Maximized);

            xnaImage.NTSC = false;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            menuStrip1.Visible = false;
#if DEBUG
            debugBar.Visible = false;
#endif
        }

        #region NTSC Submenu
        private void customNtscForm_ApplyFromForm()
        {
            AutosaveConfig();
        }

        #region Button Click event of NTSC options
        private void ntscCustom_Click(object sender, EventArgs e)
        {
            customNtscForm.Location = new Point(this.Location.X + (this.Size.Width - customNtscForm.Size.Width) / 2, this.Location.Y + (this.Size.Height - customNtscForm.Size.Height) / 2);
            customNtscForm.TopMost = this.TopMost;
            customNtscForm.ShowDialog();
            customNtscForm.TopMost = false;
        }
        #endregion
        #endregion
        #endregion

        #region Third Section
        #region Code for Hide Menu
        private void hideMenu(bool hideMenu)
        {
            if (hideMenu && menuStrip1.Visible)
            {
                hideMenuItem.Checked = true;
                Height -= menuStrip1.Height;
                menuStrip1.Visible = false;
            }
            else if (!hideMenu && !menuStrip1.Visible)
            {
                hideMenuItem.Checked = false;
                menuStrip1.Visible = true;
                Height += menuStrip1.Height;
            }
        }

        private void hideMenuItem_Click(object sender, EventArgs e)
        {
            hideMenu(!hideMenuItem.Checked);
        }
        #endregion

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
        #endregion
        #endregion
        
        #region Audio Menu
        #region First Section
        private void setMusic(bool value)
        {
            Engine.Instance.SoundSystem.MusicEnabled = musicMenuItem.Checked = value;
        }

        private void musicMenuItem_Click(object sender, EventArgs e)
        {
            setMusic(!musicMenuItem.Checked);
        }

        private void setSFX(bool value)
        {
            Engine.Instance.SoundSystem.SfxEnabled = sfxMenuItem.Checked = value;
        }

        private void sfxMenuItem_Click(object sender, EventArgs e)
        {
            setSFX(!sfxMenuItem.Checked);
        }
        #endregion
        
        public void SetVolume(int value)
        {
            Engine.Instance.SoundSystem.Volume = value;
        }

        private void increaseVolumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetVolume(Engine.Instance.SoundSystem.Volume + 1);
        }

        private void decreaseVolumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetVolume(Engine.Instance.SoundSystem.Volume - 1);
        }

        #endregion

        #region Debug Menu

        #region Debug Menu
        #region First Section
        private void setDebugBar(bool value)
        {
            debugBarToolStripMenuItem.Checked = value;
            if (debugBar.Visible == value) return;

            debugBar.Visible = value;
            Height += debugBar.Height * (debugBar.Visible ? 1 : -1);
        }

        private void debugBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDebugBar(!debugBarToolStripMenuItem.Checked);
        }
        #endregion

        #region Second Section
        private void setShowHitBoxes(bool value)
        {
            showHitboxesToolStripMenuItem.Checked = Engine.Instance.DrawHitboxes = value;
        }

        private void showHitboxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setShowHitBoxes(!showHitboxesToolStripMenuItem.Checked);
        }

        #region Cheat Submenu
        private void SetNoDamage(bool value)
        {
            noDamageToolStripMenuItem.Checked = Engine.Instance.NoDamage = value;
        }

        private void noDamageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNoDamage(!noDamageToolStripMenuItem.Checked);
        }

        private void setInvincibility(bool value)
        {
            invincibilityToolStripMenuItem.Checked = Engine.Instance.Invincible = value;
        }

        private void invincibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setInvincibility(!invincibilityToolStripMenuItem.Checked);
        }

#if DEBUG
        private void gravityFlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame == null) return;
            gravityFlipToolStripMenuItem.Checked = Game.CurrentGame.DebugFlipGravity();
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
#endif
        #endregion

        #endregion

        #region Third Section
        
        private void SetFrameRate(int framerate)
        {
            Engine.Instance.FPS = framerate;
            fpsCapLabel.Text = "FPS Cap: " + Engine.Instance.FPS;
        }

        private void framerateUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFrameRate(Engine.Instance.FPS + 10);
        }

        private void framerateDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFrameRate(Engine.Instance.FPS - 10);
        }

        private void defaultFramerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFrameRate(UserSettings.Default.Debug.Framerate);
        }

        #endregion
        #endregion

        #endregion
        #endregion
        
        private bool LoadGame(string path, List<string> pathArgs = null, bool silenceErrorMessages = false)
        {
            try
            {
                Game.Load(path, pathArgs);
                Text = Game.CurrentGame.Name;

                lastGameWithPath = path;
                LoadCurrentConfig();

                OnGameLoadedChanged();

                var userSettings = this.settingsService.GetSettings();
                userSettings.AddRecentGame(Game.CurrentGame.Name, path);
                XML.SaveToConfigXML(userSettings, this.settingsService.SettingsFilePath);

                return true;
            }
            catch (GameXmlException ex)
            {
                if (silenceErrorMessages == false)
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
                }
                Game.CurrentGame.Unload();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                if (silenceErrorMessages == false)
                {
                    MessageBox.Show("I'm sorry, I couldn't the following file. Perhaps the file path is incorrect?\n\n" + ex.Message, "C# MegaMan Engine", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Game.CurrentGame.Unload();
            }
            catch (XmlException ex)
            {
                if (silenceErrorMessages == false)
                {
                    MessageBox.Show("Your XML is badly formatted.\n\nFile: " + ex.SourceUri + "\n\nError: " + ex.Message, "C# MegaMan Engine", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Game.CurrentGame.Unload();
            }
            catch (Exception ex)
            {
                if (silenceErrorMessages == false)
                {
                    MessageBox.Show("There was an error loading the game.\n\n" + ex.Message, "C# MegaMan Engine", MessageBoxButtons.OK, MessageBoxIcon.Error);

#if DEBUG
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();

                    MessageBox.Show("StackTrace: " + st + " Frame: " + frame + " Line: " + line, "C# MegaMan Engine", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                }
                Game.CurrentGame.Unload();
            }

            // Only call if if current form is the active one
            if ((IntPtr)GetForegroundWindow() == this.Handle)
            {
                this.OnActivated(new EventArgs());
            }

            return false;
        }
        
        /// <summary>
        /// Function used when loading a game.
        /// When loading a new game, it needs to be set again because those properties are reset.
        /// </summary>
        private void SetLayersVisibilityFromSettings()
        {
            Engine.Instance.SetLayerVisibility(0, backgroundToolStripMenuItem.Checked);
            Engine.Instance.SetLayerVisibility(1, sprites1ToolStripMenuItem.Checked);
            Engine.Instance.SetLayerVisibility(2, sprites2ToolStripMenuItem.Checked);
            Engine.Instance.SetLayerVisibility(3, sprites3ToolStripMenuItem.Checked);
            Engine.Instance.SetLayerVisibility(4, sprites4ToolStripMenuItem.Checked);
            Engine.Instance.SetLayerVisibility(5, foregroundToolStripMenuItem.Checked);
        }

        /// <summary>
        /// This is kind of a bad patch but found no better way to do it.
        /// OnMove function is used when user is moving the form, and OnResizeEnd is used for when he finish.
        /// However, when moving it with coordinates, OnMove is called, but not OnResizeEnd.
        /// So we restart the engine after the move if it was running.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        private void ChangeFormLocation(int X, int Y)
        {
            bool running = Engine.Instance.IsRunning;

            if (X < 0 || Y < 0)
            {
                this.CenterToScreen();
            }
            else this.Location = new System.Drawing.Point(X, Y);

            if (running) OnResizeCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>Does no valiation, if crashes just doesn't set param</remarks>
        private void LoadGlobalConfigValues()
        {
            try
            {
                var userSettings = this.settingsService.GetSettings();

                autosaveToolStripMenuItem.Checked = userSettings.AutosaveSettings;
                defaultConfigToolStripMenuItem.Checked = userSettings.UseDefaultSettings;
                autoloadToolStripMenuItem.Checked = lastGameWithPath == userSettings.Autoload ? true : false;
                initialFolder = userSettings.InitialFolder;

                openRecentToolStripMenuItem.DropDownItems.Clear();
                foreach (var recent in userSettings.RecentGames)
                {
                    var path = recent.Path;
                    openRecentToolStripMenuItem.DropDownItems.Add(recent.Name, null, (s, e) => {
                        LoadGame(path);
                    });
                }
            }
            catch (Exception) { }
        }

        private void LoadConfigFromSetting(Setting settings)
        {
            #region Input Menu: Keys
            foreach (var binding in settings.KeyBindings)
            {
                GameInput.AddBinding(binding.GetGameInputBinding());
            }
            foreach (var binding in settings.JoystickBindings)
            {
                GameInput.AddBinding(binding.GetGameInputBinding());
            }
            foreach (var binding in settings.GamepadBindings)
            {
                GameInput.AddBinding(binding.GetGameInputBinding());
            }
            GameInput.ActiveType = settings.ActiveInput;
            #endregion

            #region Screen Menu
            // NTSC option is set before. So if menu selected is NTSC, options are set.
            if (!Enum.IsDefined(typeof(NTSC_Options), settings.Screens.NTSC_Options))
            {
                WrongConfigAlert(ConfigFileInvalidValuesMessages.NTSC_Option);
                settings.Screens.NTSC_Options = UserSettings.Default.Screens.NTSC_Options;
            }
            
            xnaImage.ntscInit(new snes_ntsc_setup_t(settings.Screens.NTSC_Custom));
            customNtscForm.SetOptions(settings.Screens.NTSC_Custom);

            if (!Enum.IsDefined(typeof(ScreenScale), settings.Screens.Size))
            {
                WrongConfigAlert(ConfigFileInvalidValuesMessages.Size);
                settings.Screens.Size = UserSettings.Default.Screens.Size;
            }

            if (!Enum.IsDefined(typeof(PixellatedOrSmoothed), settings.Screens.Pixellated))
            {
                WrongConfigAlert(ConfigFileInvalidValuesMessages.PixellatedOrSmoothed);
                settings.Screens.Pixellated = UserSettings.Default.Screens.Pixellated;
            }

            hideMenu(settings.Screens.HideMenu);

            if (settings.Screens.Maximized) WindowState = FormWindowState.Maximized;
            #endregion

            #region Audio Menu
            SetVolume(settings.Audio.Volume);
            setMusic(settings.Audio.Musics);
            setSFX(settings.Audio.Sound);
            #endregion

            #region Debug Menu
#if DEBUG
            setDebugBar(settings.Debug.ShowMenu);
            setShowHitBoxes(settings.Debug.ShowHitboxes);
            SetFrameRate(settings.Debug.Framerate);

            #region Cheats
            setInvincibility(settings.Debug.Cheat.Invincibility);
            SetNoDamage(settings.Debug.Cheat.NoDamage);
            #endregion
#else
            setDebugBar(UserSettings.Default.Debug.ShowMenu);
            setShowHitBoxes(UserSettings.Default.Debug.ShowHitboxes);
            SetFrameRate(UserSettings.Default.Debug.Framerate);

            #region Cheats
            setInvincibility(UserSettings.Default.Debug.Cheat.Invincibility);
            SetNoDamage(UserSettings.Default.Debug.Cheat.NoDamage);
            #endregion
#endif

            #endregion

            #region Miscellaneous
            ChangeFormLocation(settings.Miscellaneous.ScreenX_Coordinate, settings.Miscellaneous.ScreenY_Coordinate);
            #endregion

            foreach (var c in this.controllers)
                c.LoadSettings(settings);
        }

        private void SaveGlobalConfigValues(string fileName = null)
        {
            var userSettings = this.settingsService.GetSettings();

            userSettings.AutosaveSettings = autosaveToolStripMenuItem.Checked;
            userSettings.UseDefaultSettings = defaultConfigToolStripMenuItem.Checked;
            userSettings.Autoload = autoloadToolStripMenuItem.Checked ? lastGameWithPath : null;
            userSettings.InitialFolder = initialFolder;

            XML.SaveToConfigXML(userSettings, this.settingsService.SettingsFilePath, fileName);
        }

        private void AutosaveConfig(string fileName = null)
        {
            if (autosaveToolStripMenuItem.Checked) SaveConfig();
        }

        /// <summary>
        /// When engine is opening, every cases of bad files are handled, then file is locked.
        /// It means that calling this function after this call, it will always be valid since only program can modify it.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="settings"></param>
        private void SaveConfig(string fileName = null, Setting settings = null)
        {
            if (settings == null)
            {
                settings = new Setting()
                {
                    GameFileName = defaultConfigToolStripMenuItem.Checked ? "" : CurrentGamePath,
                    GameTitle = defaultConfigToolStripMenuItem.Checked ? "" : CurrentGameTitle,
                    KeyBindings = GetKeyBindingSettings(),
                    JoystickBindings = GetJoystickBindingSettings(),
                    GamepadBindings = GetGamepadBindingSettings(),
                    ActiveInput = GameInput.ActiveType,
                    Screens = new LastScreen()
                    {
                        Maximized = WindowState == FormWindowState.Maximized,
                        NTSC_Custom = customNtscForm.GetOptions(),
                        HideMenu = hideMenuItem.Checked
                    },
                    Audio = new LastAudio()
                    {
                        Volume = Engine.Instance.SoundSystem.Volume,
                        Musics = musicMenuItem.Checked,
                        Sound = sfxMenuItem.Checked
                    },
                    Debug = new LastDebug()
                    {
                        ShowMenu = debugBarToolStripMenuItem.Checked,
                        ShowHitboxes = showHitboxesToolStripMenuItem.Checked,
                        Framerate = Engine.Instance.FPS,
                        Cheat = new LastCheat()
                        {
                            Invincibility = invincibilityToolStripMenuItem.Checked,
                            NoDamage = noDamageToolStripMenuItem.Checked
                        }
                    },
                    Miscellaneous = new LastMiscellaneous()
                    {
                        ScreenX_Coordinate = this.Location.X,
                        ScreenY_Coordinate = this.Location.Y
                    }
                };

                foreach (var c in this.controllers)
                    c.SaveSettings(settings);
            }

            var userSettings = this.settingsService.GetSettings();
            userSettings.AddOrSetExistingSettingsForGame(settings);

            XML.SaveToConfigXML(userSettings, this.settingsService.SettingsFilePath, fileName);
        }

        private List<UserKeyBindingSetting> GetKeyBindingSettings()
        {
            return GameInput.GetKeyBindings().Select(x => new UserKeyBindingSetting() { Input = x.Input, Key = x.Key }).ToList();
        }

        private List<UserJoystickBindingSetting> GetJoystickBindingSettings()
        {
            return GameInput.GetJoystickBindings().Select(x => new UserJoystickBindingSetting() { Input = x.Input, DeviceGuid = x.DeviceGuid, Button = x.Button, Value = x.Value }).ToList();
        }

        private List<UserGamepadBindingSetting> GetGamepadBindingSettings()
        {
            return GameInput.GetGamepadBindings().Select(x => new UserGamepadBindingSetting() { Input = x.Input, Button = x.Button }).ToList();
        }

        private void Engine_Exception(Exception e)
        {
            MessageBox.Show(this, e.Message, "Game Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            CloseGame();
        }

        private void Programming_Error_No_Shutdown(string message)
        {
            MessageBox.Show(this, message, "Programming Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        }

        private void WrongConfigAlert(string message)
        {
            MessageBox.Show(this, message, "Config File Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        }
        
        /// <summary>
        /// Anytime game loaded is changed
        /// </summary>
        private void OnGameLoadedChanged()
        {
            LoadGlobalConfigValues();
            OnGameLoaded();
        }

        /// <summary>
        /// Things to do when a game is loaded
        /// </summary>
        private void OnGameLoaded()
        {
            SetLayersVisibilityFromSettings();
        }

        private void DefaultScreen()
        {
            width = Const.PixelsAcross;
            height = Const.PixelsDown;

            ResizeScreen();
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

            int extraHeight = Height - xnaImage.Height;
            int extraWidth = Width - xnaImage.Width;

            Height = extraHeight + newHeight.Value;
            Width = extraWidth + newWidth.Value;
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
    }
}
