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
        #region Variables And Constants
        #region Variables
        private string settingsPath, currentGame;
        private int widthZoom, heightZoom, width, height;
        private bool fullScreenToolStripMenuItem_IsMaximized, useGlobalConfig; // useGlobalConfig: if false, used a config specific to a game name in xml file for configs.
        
        private bool menu, altKeyDown, gotFocus; // menu is either used when context menu or title bar menu is opened
                                                 // altKeyDown is exclusively used to know if it is the menu bar is activated by alt key

        ToolStripMenuItem previousScreenSizeSelection; // Remember previous screen selection to fullscreen option. Then when fullscreen is quitted, it goes back to this option

        public static bool pauseEngine;
        #endregion

        #region Constants
        private readonly CustomNtscForm customNtscForm = new CustomNtscForm();

        #region Code used by windows messages
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_INITMENUPOPUP = 0x0117;
        private const int WM_UNINITMENUPOPUP = 0x0125;
        #endregion
        #endregion
        #endregion

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

        #region Form Events Openings/Closings
        public MainForm()
        {
            InitializeComponent();

            menu = gotFocus = altKeyDown = false;
            defaultConfigToolStripMenuItem.Checked = useGlobalConfig = true;
            currentGame = "";

#if !DEBUG
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
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            
            try
            {
                LoadConfig();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message); // If a line in config file is wrong, this is gonna tell user.
                MessageBox.Show("The config file could was not loaded successfully.");
            }
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

        protected override void OnClosed(EventArgs e)
        {
            SaveConfig();
            base.OnClosed(e);
        }
        #endregion

        #region Menus
        #region File Menu
        #region First section
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                pauseOff();

                LoadGame(dialog.FileName);
                dialog.FileName = "";
                SetLayersVisibilityFromSettings();
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

        #region Close Game
        private void CloseGame()
        {
            if (Game.CurrentGame != null)
            {
                Game.CurrentGame.Unload();
                this.xnaImage.Clear();
                Text = "Mega Man";
            }
        }

        private void closeGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentGame = "";
            CloseGame();
        }
        #endregion

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.CurrentGame != null) Game.CurrentGame.Unload();
            SaveConfig();
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
        #endregion

        #region Third Section
        private void saveConfigurationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void defaultConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveConfig();
            defaultConfigToolStripMenuItem.Checked = useGlobalConfig = !useGlobalConfig;
        }
        #endregion
        #endregion
        
        #region Input Menu
        private void keyboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Keyboard keyform = new Keyboard();
            keyform.Show();
        }
        #endregion

        #region Screen Menu
        #region First Section
        private void AllScreenResolutionOffBut(ToolStripMenuItem itemToKeepChecked)
        {
            fullScreenToolStripMenuItem.Checked = screenNTSCMenu.Checked = false;
            screen1XMenu.Checked = screen2XMenu.Checked = screen3XMenu.Checked = screen4XMenu.Checked = false;

            itemToKeepChecked.Checked = true;
        }
        #region Size Selection
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

        /// <summary>
        /// For screen option X1, X2, X3, X4, they use mostly similar code.
        /// </summary>
        /// <param name="index"></param>
        private void screenSizeMenuSelected(int index)
        {
            if (index == (Int16)UserSettingsEnums.Screen.NTSC)
            {
                if (ntscComposite.Checked) ntscOptionSet((Int16)UserSettingsEnums.NTSC_Options.Composite);
                else if (ntscComposite.Checked) ntscOptionSet((Int16)UserSettingsEnums.NTSC_Options.RGB);
                else if (ntscComposite.Checked) ntscOptionSet((Int16)UserSettingsEnums.NTSC_Options.S_Video);
                //if (ntscComposite.Checked) ntscOptionSet((Int16)UserSettingsEnums.NTSC_Options.Composite);
            }
            if (index == (Int16)UserSettingsEnums.Screen.X1)
            {
                previousScreenSizeSelection = screen1XMenu;
                widthZoom = heightZoom = 1;
            }
            if (index == (Int16)UserSettingsEnums.Screen.X2)
            {
                previousScreenSizeSelection = screen2XMenu;
                widthZoom = heightZoom = 2;
            }
            if (index == (Int16)UserSettingsEnums.Screen.X3)
            {
                previousScreenSizeSelection = screen3XMenu;
                widthZoom = heightZoom = 3;
            }
            if (index == (Int16)UserSettingsEnums.Screen.X4)
            {
                previousScreenSizeSelection = screen4XMenu;
                widthZoom = heightZoom = 4;
            }
            ScreenSizeMultiple();
            AllScreenResolutionOffBut(previousScreenSizeSelection);
        }

        private void screen1XMenu_Click(object sender, EventArgs e)
        {
            screenSizeMenuSelected((Int16)UserSettingsEnums.Screen.X1);
        }

        private void screen2XMenu_Click(object sender, EventArgs e)
        {
            screenSizeMenuSelected((Int16)UserSettingsEnums.Screen.X2);
        }

        private void screen3XMenu_Click(object sender, EventArgs e)
        {
            screenSizeMenuSelected((Int16)UserSettingsEnums.Screen.X3);
        }

        private void screen4XMenu_Click(object sender, EventArgs e)
        {
            screenSizeMenuSelected((Int16)UserSettingsEnums.Screen.X4);
        }

        #region NTSC Menu Clicked
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
            screenSizeMenuSelected((Int16)UserSettingsEnums.Screen.NTSC);
        }
        #endregion
        #endregion

        #region NTSC Submenu
        #region NTSC Custom Functions
        /// <summary>
        /// Functions that preparer parameters to call customNtscForm_Apply from parameters in a wrapper class
        /// Receive all the values in a wrapper class
        /// </summary>
        /// <param name="values"></param>
        /// <remarks>Used to set values from config file read.</remarks>
        private void customNtscForm_ApplyFromWrapperObject(NTSC_CustomOptions wrapper)
        {
            ntscOptionCode(ntscCustom, new snes_ntsc_setup_t(wrapper.Hue, wrapper.Saturation, wrapper.Contrast, wrapper.Brightness,
                wrapper.Sharpness, wrapper.Gamma, wrapper.Resolution, wrapper.Artifacts, wrapper.Fringing, wrapper.Bleed, true));
        }

        /// <summary>
        /// Functions that preparer parameters to call customNtscForm_Apply from form settings
        /// Build a snes_ntsc_setup_t item from form parameters
        /// </summary>
        private void customNtscForm_ApplyFromForm()
        {
            ntscOptionCode(ntscCustom, new snes_ntsc_setup_t(customNtscForm.Hue, customNtscForm.Saturation, customNtscForm.Contrast, customNtscForm.Brightness,
                customNtscForm.Sharpness, customNtscForm.Gamma, customNtscForm.Resolution, customNtscForm.Artifacts, customNtscForm.Fringing, customNtscForm.Bleed, true));
        }
        #endregion

        #region NTSC setting the option (once snes_ntsc_setup_t variable is build)
        private void NTSC_OptionsOffBut(ToolStripMenuItem itemToKeepChecked)
        {
            ntscComposite.Checked = ntscSVideo.Checked = ntscRGB.Checked = ntscCustom.Checked = false;

            itemToKeepChecked.Checked = true;
        }

        private void ntscOptionCode(ToolStripMenuItem menuClicked, snes_ntsc_setup_t snes_ntsc_type, bool setOption = true)
        {
            NTSC_OptionsOffBut(menuClicked);

            if (setOption)
            {
                screenNTSCSelected();
                xnaImage.ntscInit(snes_ntsc_type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NTSC_Option"></param>
        /// <param name="customParameters"></param>
        /// <param name="setOption">If not set, we only check the option</param>
        private void ntscOptionSet(Int16 NTSC_Option, NTSC_CustomOptions customParameters = null, bool setOption = true)
        {
            // Set parameters of Custom options
            if (customParameters != null) customNtscForm_ApplyFromWrapperObject(customParameters);

            if (NTSC_Option == (Int16)UserSettingsEnums.NTSC_Options.Composite) ntscOptionCode(ntscComposite, snes_ntsc_setup_t.snes_ntsc_composite, setOption);
            if (NTSC_Option == (Int16)UserSettingsEnums.NTSC_Options.S_Video) ntscOptionCode(ntscSVideo, snes_ntsc_setup_t.snes_ntsc_svideo, setOption);
            if (NTSC_Option == (Int16)UserSettingsEnums.NTSC_Options.RGB) ntscOptionCode(ntscRGB, snes_ntsc_setup_t.snes_ntsc_rgb, setOption);
        }
        #endregion

        #region Button Click event of NTSC options
        private void ntscComposite_Click(object sender, EventArgs e)
        {
            ntscOptionSet((Int16)UserSettingsEnums.NTSC_Options.Composite);
        }

        private void ntscSVideo_Click(object sender, EventArgs e)
        {
            ntscOptionSet((Int16)UserSettingsEnums.NTSC_Options.S_Video);
        }

        private void ntscRGB_Click(object sender, EventArgs e)
        {
            ntscOptionSet((Int16)UserSettingsEnums.NTSC_Options.RGB);
        }

        private void ntscCustom_Click(object sender, EventArgs e)
        {
            customNtscForm.Show();
        }
        #endregion
        #endregion

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fullScreenToolStripMenuItem.Checked = !fullScreenToolStripMenuItem.Checked;

            if (fullScreenToolStripMenuItem.Checked)
            {
                fullScreenToolStripMenuItem_IsMaximized = (this.WindowState == FormWindowState.Maximized) ? true : false;

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
        #endregion

        #region Second Section
        #region Pixellated/Smoothed Section
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
        private void pixellatedVsSmoothedCode(Int32 index)
        {
            if (index == (Int32)UserSettingsEnums.PixellatedOrSmoothed.Pixellated)
            {
                Engine.Instance.FilterState = Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp;
                pixellatedVsSmoothedAllOffBut(pixellatedToolStripMenuItem);
            }
            else if (index == (Int32)UserSettingsEnums.PixellatedOrSmoothed.Smoothed)
            {
                Engine.Instance.FilterState = Microsoft.Xna.Framework.Graphics.SamplerState.LinearClamp;
                pixellatedVsSmoothedAllOffBut(smoothedToolStripMenuItem);
            }
        }

        private void smoothedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pixellatedVsSmoothedCode((Int16)UserSettingsEnums.PixellatedOrSmoothed.Smoothed);
        }

        private void pixellatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pixellatedVsSmoothedCode((Int16)UserSettingsEnums.PixellatedOrSmoothed.Pixellated);
        }
        #endregion
        #endregion

        #region Third Section
        #region Code for Hide Menu
        private void hideMenu(bool hideMenu)
        {
            if (hideMenu)
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
        #region Volume
        public void SetVolume(int value)
        {
            Engine.Instance.Volume = value;
        }

        private void increaseVolumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetVolume(Engine.Instance.Volume + 1);
        }

        private void decreaseVolumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetVolume(Engine.Instance.Volume -1);
        }
        #endregion
        #endregion

        #region Second Section
        private void setMusic(bool value)
        {
            if (!pauseEngine) Engine.Instance.SoundSystem.MusicEnabled = musicMenuItem.Checked = value;
        }

        private void musicMenuItem_Click(object sender, EventArgs e)
        {
            setMusic(!musicMenuItem.Checked);
        }

        private void setSFX(bool value)
        {
            Engine.Instance.SoundSystem.SfxEnabled = sfxMenuItem.Checked = value; ;
        }

        private void sfxMenuItem_Click(object sender, EventArgs e)
        {
            setSFX(!sfxMenuItem.Checked);
        }
        #endregion

        #region Third Section
        private void setSq1(bool value)
        {
            Engine.Instance.SoundSystem.SquareOne = sq1MenuItem.Checked = value;
        }

        private void sq1MenuItem_Click(object sender, EventArgs e)
        {
            setSq1(!sq1MenuItem.Checked);
        }

        private void setSq2(bool value)
        {
            Engine.Instance.SoundSystem.SquareTwo = sq2MenuItem.Checked = value;
        }

        private void sq2MenuItem_Click(object sender, EventArgs e)
        {
            setSq2(!sq2MenuItem.Checked);
        }

        private void setTri(bool value)
        {
            Engine.Instance.SoundSystem.Triangle = triMenuItem.Checked = value;
        }

        private void triMenuItem_Click(object sender, EventArgs e)
        {
            setTri(!triMenuItem.Checked);
        }

        private void setNoise(bool value)
        {
            Engine.Instance.SoundSystem.Noise = noiseMenuItem.Checked = value;
        }

        private void noiseMenuItem_Click(object sender, EventArgs e)
        {
            setNoise(!noiseMenuItem.Checked);
        }
        #endregion
        #endregion

        #region Debug Menu

        #region Debug Menu
        #region First Section
        private void setDebugBar(bool value)
        {
            debugBar.Visible = value;
            Height += debugBar.Height * (debugBar.Visible ? 1 : -1);
            debugBarToolStripMenuItem.Checked = debugBar.Visible;
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
        #endregion

        #region Layer Submenu
        private void setLayerVisibility(Int16 index, bool value)
        {
            if (index == (Int16)UserSettingsEnums.Layers.Background)
            {
                Engine.Instance.ToggleLayerVisibility(index);
                backgroundToolStripMenuItem.Checked = value;
            }
            else if (index == (Int16)UserSettingsEnums.Layers.Sprite1)
            {
                Engine.Instance.ToggleLayerVisibility(index);
                sprites1ToolStripMenuItem.Checked = value;
            }
            else if (index == (Int16)UserSettingsEnums.Layers.Sprite2)
            {
                Engine.Instance.ToggleLayerVisibility(index);
                sprites2ToolStripMenuItem.Checked = value;
            }
            else if (index == (Int16)UserSettingsEnums.Layers.Sprite3)
            {
                Engine.Instance.ToggleLayerVisibility(index);
                sprites3ToolStripMenuItem.Checked = value;
            }
            else if (index == (Int16)UserSettingsEnums.Layers.Sprite4)
            {
                Engine.Instance.ToggleLayerVisibility(index);
                sprites4ToolStripMenuItem.Checked = value;
            }
            else if (index == (Int16)UserSettingsEnums.Layers.Foreground)
            {
                Engine.Instance.ToggleLayerVisibility(index);
                foregroundToolStripMenuItem.Checked = value;
            }
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLayerVisibility((Int16)UserSettingsEnums.Layers.Background, !backgroundToolStripMenuItem.Checked);
        }

        private void sprites1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLayerVisibility((Int16)UserSettingsEnums.Layers.Sprite1, !sprites1ToolStripMenuItem.Checked);
        }

        private void sprites2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLayerVisibility((Int16)UserSettingsEnums.Layers.Sprite2, !sprites2ToolStripMenuItem.Checked);
        }

        private void sprites3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLayerVisibility((Int16)UserSettingsEnums.Layers.Sprite3, !sprites3ToolStripMenuItem.Checked);
        }

        private void sprites4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLayerVisibility((Int16)UserSettingsEnums.Layers.Sprite4, !sprites4ToolStripMenuItem.Checked);
        }

        private void foregroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLayerVisibility((Int16)UserSettingsEnums.Layers.Foreground, !foregroundToolStripMenuItem.Checked);
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
        #endregion
        #endregion

        #region Third Section

        #region Framerate Setting
        private void SetFrameRate(int framerate)
        {
            Engine.Instance.FPS = framerate;

            if (Engine.Instance.FPS < Constants.EngineProperties.FramerateMin) Engine.Instance.FPS = Constants.EngineProperties.FramerateMin;
            if (Engine.Instance.FPS > Constants.EngineProperties.FramerateMax) Engine.Instance.FPS = Constants.EngineProperties.FramerateMax;

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
            SetFrameRate(ConfigFilesDefaultValues.Framerate);
        }
        #endregion

        #endregion
        #endregion

        #endregion
        #endregion

        #region Functions Used By Many
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

        #region Configs Functions
        /// <summary>
        /// Returns UserSettings build from XML.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>null if error happens in process</returns>
        UserSettings GetUserSettingsFromXML(string fileName = null)
        {
            UserSettings var = null;

            try
            {
                if (fileName == null) fileName = Constants.Paths.SettingFile;

                settingsPath = Path.Combine(Application.StartupPath, fileName);
                if (File.Exists(settingsPath))
                {
                    var serializer = new XmlSerializer(typeof(UserSettings));
                    using (var file = File.Open(settingsPath, FileMode.Open))
                    {
                        var = (UserSettings)serializer.Deserialize(file);
                    }
                }
            }
            catch (Exception)
            {
                var = null;
            }

            return var;
        }

        private void LoadConfig(string fileName = null)
        {
            UserSettings settingsArray = GetUserSettingsFromXML(fileName);
            var settings = settingsArray.GetSettingsForGame();

            #region Input Menu: Keys
            GameInputKeys.Up = settings.Keys.Up;
            GameInputKeys.Down = settings.Keys.Down;
            GameInputKeys.Left = settings.Keys.Left;
            GameInputKeys.Right = settings.Keys.Right;
            GameInputKeys.Jump = settings.Keys.Jump;
            GameInputKeys.Shoot = settings.Keys.Shoot;
            GameInputKeys.Start = settings.Keys.Start;
            GameInputKeys.Select = settings.Keys.Select;
            #endregion

            #region Screen Menu
            // NTSC option is set before. So if menu selected is NTSC, options are set.
            if (!Enum.IsDefined(typeof(UserSettingsEnums.NTSC_Options), settings.Screens.NTSC_Options))
            {
                WrongConfigAlert(ConfigFileInvalidValuesMessages.NTSC_Option);
                settings.Screens.NTSC_Options = ConfigFilesDefaultValues.NTSC_Option;
            }
            ntscOptionSet((Int16)settings.Screens.NTSC_Options, settings.Screens.NTSC_Custom, false);

            if (!Enum.IsDefined(typeof(UserSettingsEnums.Screen), settings.Screens.Size))
            {
                WrongConfigAlert(ConfigFileInvalidValuesMessages.Size);
                settings.Screens.Size = ConfigFilesDefaultValues.Size;
            }
            screenSizeMenuSelected(settings.Screens.Size);

            if (!Enum.IsDefined(typeof(UserSettingsEnums.PixellatedOrSmoothed), settings.Screens.Pixellated))
            {
                WrongConfigAlert(ConfigFileInvalidValuesMessages.PixellatedOrSmoothed);
                settings.Screens.Pixellated = ConfigFilesDefaultValues.PixellatedOrSmoothed;
            }
            pixellatedVsSmoothedCode(settings.Screens.Pixellated);

            hideMenu(settings.Screens.HideMenu);

            if (settings.Screens.Maximized) WindowState = FormWindowState.Maximized;
            #endregion

            #region Audio Menu
            SetVolume(settings.Audio.Volume);
            setMusic(settings.Audio.Musics);
            setSFX(settings.Audio.Sound);
            // setSq1(settings.Audio.Square1);
            // setSq2(settings.Audio.Square2);
            // setTri(settings.Audio.Triangle);
            // setNoise(settings.Audio.Noise);
            #endregion

            #region Debug Menu
            setDebugBar(settings.Debug.ShowMenu);
            setShowHitBoxes(settings.Debug.ShowHitboxes);
            SetFrameRate(settings.Debug.Framerate);

            #region Cheats
            setInvincibility(settings.Debug.Cheat.Invincibility);
            SetNoDamage(settings.Debug.Cheat.NoDamage);
            #endregion

            #region Layers
            setLayerVisibility((Int16)UserSettingsEnums.Layers.Background, settings.Debug.Layers.Background);
            setLayerVisibility((Int16)UserSettingsEnums.Layers.Sprite1, settings.Debug.Layers.Sprites1);
            setLayerVisibility((Int16)UserSettingsEnums.Layers.Sprite2, settings.Debug.Layers.Sprites2);
            setLayerVisibility((Int16)UserSettingsEnums.Layers.Sprite3, settings.Debug.Layers.Sprites3);
            setLayerVisibility((Int16)UserSettingsEnums.Layers.Sprite4, settings.Debug.Layers.Sprites4);
            setLayerVisibility((Int16)UserSettingsEnums.Layers.Foreground, settings.Debug.Layers.Foreground);
            #endregion
            #endregion

            #region Miscellaneous
            this.Location = new System.Drawing.Point(settings.Miscellaneous.ScreenX_Coordinate, settings.Miscellaneous.ScreenY_Coordinate);
            #endregion
        }

        #region Functions to build datas for saving config
        private Int32 currentSize()
        {
            if (screen2XMenu.Checked) return (Int32)UserSettingsEnums.Screen.X2;
            if (screen3XMenu.Checked) return (Int32)UserSettingsEnums.Screen.X3;
            if (screen4XMenu.Checked) return (Int32)UserSettingsEnums.Screen.X4;
            if (screenNTSCMenu.Checked) return (Int32)UserSettingsEnums.Screen.NTSC;

            return (Int32)UserSettingsEnums.Screen.X1;
        }

        private Int32 currentNTSC_Option()
        {
            if (ntscComposite.Checked) return (Int32)UserSettingsEnums.NTSC_Options.Composite;
            if (ntscSVideo.Checked) return (Int32)UserSettingsEnums.NTSC_Options.S_Video;
            if (ntscRGB.Checked) return (Int32)UserSettingsEnums.NTSC_Options.RGB;

            return (Int32)UserSettingsEnums.NTSC_Options.Custom;
        }

        public Int32 currentPixellatedOrSmoothedOption()
        {
            if (pixellatedToolStripMenuItem.Checked) return (Int32)UserSettingsEnums.PixellatedOrSmoothed.Pixellated;
            return (Int32)UserSettingsEnums.PixellatedOrSmoothed.Smoothed;
        }
        #endregion

        private void SaveConfig(string fileName = null)
        {
            if (fileName == null) fileName = Constants.Paths.SettingFile;

            var serializer = new XmlSerializer(typeof(UserSettings));

            #region Creation of variable to save
            var settings = new Setting()
            {
                GameFileName = currentGame = "",
                Keys = new UserKeys()
                {
                    Up = GameInputKeys.Up,
                    Down = GameInputKeys.Down,
                    Left = GameInputKeys.Left,
                    Right = GameInputKeys.Right,
                    Jump = GameInputKeys.Jump,
                    Shoot = GameInputKeys.Shoot,
                    Start = GameInputKeys.Start,
                    Select = GameInputKeys.Select
                },
                Screens = new LastScreen()
                {
                    Size = currentSize(),
                    Maximized = WindowState == FormWindowState.Maximized ? true : false,
                    NTSC_Options = currentNTSC_Option(),
                    NTSC_Custom = new NTSC_CustomOptions()
                    {
                        // !!!NTSC option write
                        Hue = 1,
                        Saturation = 1,
                        Brightness = 1,
                        Contrast = 1,
                        Sharpness = 1,
                        Gamma = 1,
                        Resolution = 1,
                        Artifacts = 1,
                        Fringing = 1,
                        Bleed = 1
                    },
                    Pixellated = currentPixellatedOrSmoothedOption(),
                    HideMenu = hideMenuItem.Checked
                },
                Audio = new LastAudio()
                {
                    Volume = Engine.Instance.Volume,
                    Musics = musicMenuItem.Checked,
                    Sound = sfxMenuItem.Checked,
                    Square1 = sq1MenuItem.Checked,
                    Square2 = sq2MenuItem.Checked,
                    Triangle = triMenuItem.Checked,
                    Noise = noiseMenuItem.Checked
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
                    },
                    Layers = new LastBackground()
                    {
                        Background = backgroundToolStripMenuItem.Checked,
                        Sprites1 = sprites1ToolStripMenuItem.Checked,
                        Sprites2 = sprites2ToolStripMenuItem.Checked,
                        Sprites3 = sprites3ToolStripMenuItem.Checked,
                        Sprites4 = sprites4ToolStripMenuItem.Checked,
                        Foreground = foregroundToolStripMenuItem.Checked
                    }
                },
                Miscellaneous = new LastMiscellaneous()
                {
                    ScreenX_Coordinate = this.Location.X,
                    ScreenY_Coordinate = this.Location.Y
                }
            };
            #endregion

            // This functions updates settingsPath
            UserSettings userSettings = GetUserSettingsFromXML(fileName);
            userSettings.AddOrSetExistingSettingsForGame(settings, currentGame);

            XmlTextWriter writer = new XmlTextWriter(settingsPath, null)
            {
                Indentation = 1,
                IndentChar = '\t',
                Formatting = Formatting.Indented
            };

            serializer.Serialize(writer, userSettings);

            writer.Close();
        }
        #endregion

        #region Errors
        private void Engine_Exception(Exception e)
        {
            MessageBox.Show(this, e.Message, "Game Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            CloseGame();
        }

        private void WrongConfigAlert(string message)
        {
            MessageBox.Show(this, message, "Config File Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        }
        #endregion
        #endregion

        #region To sort!!!!!!
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
        #endregion
    }
}