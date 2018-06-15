using System.ComponentModel;
using System.Windows.Forms;

namespace MegaMan.Engine
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.pauseEngineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.autosaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConfigurationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteConfigurationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.autoloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.audioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.musicMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sfxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.sq1MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sq2MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noiseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.increaseVolumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decreaseVolumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screen1XMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.screen2XMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.screen3XMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.screen4XMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.screenNTSCMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ntscComposite = new System.Windows.Forms.ToolStripMenuItem();
            this.ntscSVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.ntscRGB = new System.Windows.Forms.ToolStripMenuItem();
            this.ntscCustom = new System.Windows.Forms.ToolStripMenuItem();
            this.fullScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pixellatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smoothedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.hideMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenshotMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.showHitboxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugCheatMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.noDamageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invincibilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gravityFlipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emptyHealthMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fillHealthMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emptyWeaponMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fillWeaponMenuIem = new System.Windows.Forms.ToolStripMenuItem();
            this.layersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sprites1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sprites2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sprites3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sprites4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foregroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.activateAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.framerateUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.framerateDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultFramerateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugBar = new System.Windows.Forms.StatusStrip();
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.thinkLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.entityLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.fpsCapLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.xnaImage = new MegaMan.Engine.EngineGraphicsControl();
            this.openRecentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.debugBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.inputToolStripMenuItem,
            this.audioToolStripMenuItem,
            this.screenToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(392, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.MenuActivate += new System.EventHandler(this.menuStrip1_MenuActivate);
            this.menuStrip1.MenuDeactivate += new System.EventHandler(this.menuStrip1_MenuDeactivate);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.openRecentToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.closeGameToolStripMenuItem,
            this.quitToolStripMenuItem,
            this.toolStripSeparator7,
            this.pauseEngineToolStripMenuItem,
            this.toolStripSeparator9,
            this.autosaveToolStripMenuItem,
            this.defaultConfigToolStripMenuItem,
            this.loadConfigurationToolStripMenuItem,
            this.saveConfigurationsToolStripMenuItem,
            this.deleteConfigurationsToolStripMenuItem,
            this.toolStripSeparator10,
            this.autoloadToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.DropDownOpened += new System.EventHandler(this.StripMenuItem_DropDownOpened);
            this.fileToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.menuStrip_MouseDown);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.loadToolStripMenuItem.Text = "Open Game";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // closeGameToolStripMenuItem
            // 
            this.closeGameToolStripMenuItem.Name = "closeGameToolStripMenuItem";
            this.closeGameToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.closeGameToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.closeGameToolStripMenuItem.Text = "Close Game";
            this.closeGameToolStripMenuItem.Click += new System.EventHandler(this.closeGameToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(303, 6);
            // 
            // pauseEngineToolStripMenuItem
            // 
            this.pauseEngineToolStripMenuItem.Name = "pauseEngineToolStripMenuItem";
            this.pauseEngineToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.pauseEngineToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.pauseEngineToolStripMenuItem.Text = "Pause Engine";
            this.pauseEngineToolStripMenuItem.Click += new System.EventHandler(this.pauseEngineToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(303, 6);
            // 
            // autosaveToolStripMenuItem
            // 
            this.autosaveToolStripMenuItem.Name = "autosaveToolStripMenuItem";
            this.autosaveToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.autosaveToolStripMenuItem.Text = "Autosave Configurations";
            this.autosaveToolStripMenuItem.Click += new System.EventHandler(this.autosaveToolStripMenuItem_Click);
            // 
            // defaultConfigToolStripMenuItem
            // 
            this.defaultConfigToolStripMenuItem.Name = "defaultConfigToolStripMenuItem";
            this.defaultConfigToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.defaultConfigToolStripMenuItem.Text = "Default Configurations";
            this.defaultConfigToolStripMenuItem.Click += new System.EventHandler(this.defaultConfigToolStripMenuItem_Click);
            // 
            // loadConfigurationToolStripMenuItem
            // 
            this.loadConfigurationToolStripMenuItem.Name = "loadConfigurationToolStripMenuItem";
            this.loadConfigurationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.loadConfigurationToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.loadConfigurationToolStripMenuItem.Text = "Load Configuration";
            this.loadConfigurationToolStripMenuItem.Click += new System.EventHandler(this.loadConfigurationToolStripMenuItem_Click);
            // 
            // saveConfigurationsToolStripMenuItem
            // 
            this.saveConfigurationsToolStripMenuItem.Name = "saveConfigurationsToolStripMenuItem";
            this.saveConfigurationsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveConfigurationsToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.saveConfigurationsToolStripMenuItem.Text = "Save Configurations";
            this.saveConfigurationsToolStripMenuItem.Click += new System.EventHandler(this.saveConfigurationsToolStripMenuItem_Click);
            // 
            // deleteConfigurationsToolStripMenuItem
            // 
            this.deleteConfigurationsToolStripMenuItem.Name = "deleteConfigurationsToolStripMenuItem";
            this.deleteConfigurationsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.deleteConfigurationsToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.deleteConfigurationsToolStripMenuItem.Text = "Clear Configurations";
            this.deleteConfigurationsToolStripMenuItem.Click += new System.EventHandler(this.deleteConfigurationsToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(303, 6);
            // 
            // autoloadToolStripMenuItem
            // 
            this.autoloadToolStripMenuItem.Checked = true;
            this.autoloadToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoloadToolStripMenuItem.Name = "autoloadToolStripMenuItem";
            this.autoloadToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.autoloadToolStripMenuItem.Text = "Autoload";
            this.autoloadToolStripMenuItem.Click += new System.EventHandler(this.autoloadToolStripMenuItem_Click);
            // 
            // inputToolStripMenuItem
            // 
            this.inputToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keyboardToolStripMenuItem});
            this.inputToolStripMenuItem.Name = "inputToolStripMenuItem";
            this.inputToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.inputToolStripMenuItem.Text = "&Input";
            this.inputToolStripMenuItem.DropDownOpened += new System.EventHandler(this.StripMenuItem_DropDownOpened);
            this.inputToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.menuStrip_MouseDown);
            // 
            // keyboardToolStripMenuItem
            // 
            this.keyboardToolStripMenuItem.Name = "keyboardToolStripMenuItem";
            this.keyboardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.K)));
            this.keyboardToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.keyboardToolStripMenuItem.Text = "Bindings...";

            this.keyboardToolStripMenuItem.Click += new System.EventHandler(this.keyboardToolStripMenuItem_Click);
            // 
            // audioToolStripMenuItem
            // 
            this.audioToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.musicMenuItem,
            this.sfxMenuItem,
            this.toolStripSeparator8,
            this.sq1MenuItem,
            this.sq2MenuItem,
            this.triMenuItem,
            this.noiseMenuItem,
            this.toolStripSeparator3,
            this.increaseVolumeToolStripMenuItem,
            this.decreaseVolumeToolStripMenuItem});
            this.audioToolStripMenuItem.Name = "audioToolStripMenuItem";
            this.audioToolStripMenuItem.Size = new System.Drawing.Size(61, 24);
            this.audioToolStripMenuItem.Text = "&Audio";
            this.audioToolStripMenuItem.DropDownOpened += new System.EventHandler(this.StripMenuItem_DropDownOpened);
            this.audioToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.menuStrip_MouseDown);
            // 
            // musicMenuItem
            // 
            this.musicMenuItem.Name = "musicMenuItem";
            this.musicMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.musicMenuItem.Size = new System.Drawing.Size(262, 22);
            this.musicMenuItem.Text = "Toggle Music";
            this.musicMenuItem.Click += new System.EventHandler(this.musicMenuItem_Click);
            // 
            // sfxMenuItem
            // 
            this.sfxMenuItem.Name = "sfxMenuItem";
            this.sfxMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.M)));
            this.sfxMenuItem.Size = new System.Drawing.Size(262, 22);
            this.sfxMenuItem.Text = "Toggle Sound Effects";
            this.sfxMenuItem.Click += new System.EventHandler(this.sfxMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(259, 6);
            // 
            // sq1MenuItem
            // 
            this.sq1MenuItem.Name = "sq1MenuItem";
            this.sq1MenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D7)));
            this.sq1MenuItem.Size = new System.Drawing.Size(262, 22);
            this.sq1MenuItem.Text = "Toggle Square 1";
            // 
            // sq2MenuItem
            // 
            this.sq2MenuItem.Name = "sq2MenuItem";
            this.sq2MenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D8)));
            this.sq2MenuItem.Size = new System.Drawing.Size(262, 22);
            this.sq2MenuItem.Text = "Toggle Square 2";
            // 
            // triMenuItem
            // 
            this.triMenuItem.Name = "triMenuItem";
            this.triMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D9)));
            this.triMenuItem.Size = new System.Drawing.Size(262, 22);
            this.triMenuItem.Text = "Toggle Triangle";
            // 
            // noiseMenuItem
            // 
            this.noiseMenuItem.Name = "noiseMenuItem";
            this.noiseMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D0)));
            this.noiseMenuItem.Size = new System.Drawing.Size(262, 22);
            this.noiseMenuItem.Text = "Toggle Noise";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(259, 6);
            // 
            // increaseVolumeToolStripMenuItem
            // 
            this.increaseVolumeToolStripMenuItem.Name = "increaseVolumeToolStripMenuItem";
            this.increaseVolumeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.increaseVolumeToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.increaseVolumeToolStripMenuItem.Text = "Increase Volume";
            this.increaseVolumeToolStripMenuItem.Click += new System.EventHandler(this.increaseVolumeToolStripMenuItem_Click);
            // 
            // decreaseVolumeToolStripMenuItem
            // 
            this.decreaseVolumeToolStripMenuItem.Name = "decreaseVolumeToolStripMenuItem";
            this.decreaseVolumeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.V)));
            this.decreaseVolumeToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.decreaseVolumeToolStripMenuItem.Text = "Decrease Volume";
            this.decreaseVolumeToolStripMenuItem.Click += new System.EventHandler(this.decreaseVolumeToolStripMenuItem_Click);
            // 
            // screenToolStripMenuItem
            // 
            this.screenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.screen1XMenu,
            this.screen2XMenu,
            this.screen3XMenu,
            this.screen4XMenu,
            this.screenNTSCMenu,
            this.fullScreenToolStripMenuItem,
            this.toolStripSeparator1,
            this.pixellatedToolStripMenuItem,
            this.smoothedToolStripMenuItem,
            this.toolStripSeparator4,
            this.hideMenuItem,
            this.screenshotMenuItem});
            this.screenToolStripMenuItem.Name = "screenToolStripMenuItem";
            this.screenToolStripMenuItem.Size = new System.Drawing.Size(65, 24);
            this.screenToolStripMenuItem.Text = "&Screen";
            this.screenToolStripMenuItem.DropDownOpened += new System.EventHandler(this.StripMenuItem_DropDownOpened);
            this.screenToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.menuStrip_MouseDown);
            // 
            // screen1XMenu
            // 
            this.screen1XMenu.Name = "screen1XMenu";
            this.screen1XMenu.Size = new System.Drawing.Size(216, 26);
            this.screen1XMenu.Text = "1x";
            // 
            // screen2XMenu
            // 
            this.screen2XMenu.Name = "screen2XMenu";
            this.screen2XMenu.Size = new System.Drawing.Size(216, 26);
            this.screen2XMenu.Text = "2x";
            // 
            // screen3XMenu
            // 
            this.screen3XMenu.Name = "screen3XMenu";
            this.screen3XMenu.Size = new System.Drawing.Size(216, 26);
            this.screen3XMenu.Text = "3x";
            // 
            // screen4XMenu
            // 
            this.screen4XMenu.Name = "screen4XMenu";
            this.screen4XMenu.Size = new System.Drawing.Size(216, 26);
            this.screen4XMenu.Text = "4x";
            // 
            // screenNTSCMenu
            // 
            this.screenNTSCMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ntscComposite,
            this.ntscSVideo,
            this.ntscRGB,
            this.ntscCustom});
            this.screenNTSCMenu.Name = "screenNTSCMenu";
            this.screenNTSCMenu.Size = new System.Drawing.Size(216, 26);
            this.screenNTSCMenu.Text = "NTSC";
            // 
            // ntscComposite
            // 
            this.ntscComposite.Name = "ntscComposite";
            this.ntscComposite.Size = new System.Drawing.Size(156, 26);
            this.ntscComposite.Text = "Composite";
            // 
            // ntscSVideo
            // 
            this.ntscSVideo.Name = "ntscSVideo";
            this.ntscSVideo.Size = new System.Drawing.Size(156, 26);
            this.ntscSVideo.Text = "S-Video";
            // 
            // ntscRGB
            // 
            this.ntscRGB.Name = "ntscRGB";
            this.ntscRGB.Size = new System.Drawing.Size(156, 26);
            this.ntscRGB.Text = "RGB";
            // 
            // ntscCustom
            // 
            this.ntscCustom.Name = "ntscCustom";
            this.ntscCustom.Size = new System.Drawing.Size(156, 26);
            this.ntscCustom.Text = "Custom ...";
            this.ntscCustom.Click += new System.EventHandler(this.ntscCustom_Click);
            // 
            // fullScreenToolStripMenuItem
            // 
            this.fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            this.fullScreenToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.fullScreenToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.fullScreenToolStripMenuItem.Text = "Full Screen";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(213, 6);
            // 
            // pixellatedToolStripMenuItem
            // 
            this.pixellatedToolStripMenuItem.Name = "pixellatedToolStripMenuItem";
            this.pixellatedToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.pixellatedToolStripMenuItem.Text = "Pixellated";
            // 
            // smoothedToolStripMenuItem
            // 
            this.smoothedToolStripMenuItem.Name = "smoothedToolStripMenuItem";
            this.smoothedToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.smoothedToolStripMenuItem.Text = "Smoothed";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(213, 6);
            // 
            // hideMenuItem
            // 
            this.hideMenuItem.Name = "hideMenuItem";
            this.hideMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.hideMenuItem.Size = new System.Drawing.Size(216, 26);
            this.hideMenuItem.Text = "Hide Menu";
            this.hideMenuItem.Click += new System.EventHandler(this.hideMenuItem_Click);
            // 
            // screenshotMenuItem
            // 
            this.screenshotMenuItem.Name = "screenshotMenuItem";
            this.screenshotMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.screenshotMenuItem.Size = new System.Drawing.Size(216, 26);
            this.screenshotMenuItem.Text = "Capture";
            this.screenshotMenuItem.Click += new System.EventHandler(this.screenshotMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugBarToolStripMenuItem,
            this.toolStripSeparator2,
            this.showHitboxesToolStripMenuItem,
            this.debugCheatMenu,
            this.layersToolStripMenuItem,
            this.toolStripSeparator6,
            this.framerateUpToolStripMenuItem,
            this.framerateDownToolStripMenuItem,
            this.defaultFramerateToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            this.debugToolStripMenuItem.Text = "&Debug";
            this.debugToolStripMenuItem.DropDownOpened += new System.EventHandler(this.StripMenuItem_DropDownOpened);
            this.debugToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.menuStrip_MouseDown);
            // 
            // debugBarToolStripMenuItem
            // 
            this.debugBarToolStripMenuItem.Name = "debugBarToolStripMenuItem";
            this.debugBarToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.debugBarToolStripMenuItem.Size = new System.Drawing.Size(289, 26);
            this.debugBarToolStripMenuItem.Text = "Show &Debug Bar";
            this.debugBarToolStripMenuItem.Click += new System.EventHandler(this.debugBarToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(286, 6);
            // 
            // showHitboxesToolStripMenuItem
            // 
            this.showHitboxesToolStripMenuItem.Name = "showHitboxesToolStripMenuItem";
            this.showHitboxesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.showHitboxesToolStripMenuItem.Size = new System.Drawing.Size(289, 26);
            this.showHitboxesToolStripMenuItem.Text = "Show &Hitboxes";
            this.showHitboxesToolStripMenuItem.Click += new System.EventHandler(this.showHitboxesToolStripMenuItem_Click);
            // 
            // debugCheatMenu
            // 
            this.debugCheatMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noDamageToolStripMenuItem,
            this.invincibilityToolStripMenuItem,
            this.gravityFlipToolStripMenuItem,
            this.emptyHealthMenuItem,
            this.fillHealthMenuItem,
            this.emptyWeaponMenuItem,
            this.fillWeaponMenuIem});
            this.debugCheatMenu.Name = "debugCheatMenu";
            this.debugCheatMenu.Size = new System.Drawing.Size(289, 26);
            this.debugCheatMenu.Text = "&Cheat";
            // 
            // noDamageToolStripMenuItem
            // 
            this.noDamageToolStripMenuItem.Name = "noDamageToolStripMenuItem";
            this.noDamageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.noDamageToolStripMenuItem.Size = new System.Drawing.Size(260, 26);
            this.noDamageToolStripMenuItem.Text = "No Damage";
            this.noDamageToolStripMenuItem.Click += new System.EventHandler(this.noDamageToolStripMenuItem_Click);
            // 
            // invincibilityToolStripMenuItem
            // 
            this.invincibilityToolStripMenuItem.Name = "invincibilityToolStripMenuItem";
            this.invincibilityToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.invincibilityToolStripMenuItem.Size = new System.Drawing.Size(260, 26);
            this.invincibilityToolStripMenuItem.Text = "Invincibility";
            this.invincibilityToolStripMenuItem.Click += new System.EventHandler(this.invincibilityToolStripMenuItem_Click);
            // 
            // gravityFlipToolStripMenuItem
            // 
            this.gravityFlipToolStripMenuItem.Name = "gravityFlipToolStripMenuItem";
            this.gravityFlipToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gravityFlipToolStripMenuItem.Size = new System.Drawing.Size(260, 26);
            this.gravityFlipToolStripMenuItem.Text = "Gravity Flip";
            // 
            // emptyHealthMenuItem
            // 
            this.emptyHealthMenuItem.Name = "emptyHealthMenuItem";
            this.emptyHealthMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.emptyHealthMenuItem.Size = new System.Drawing.Size(260, 26);
            this.emptyHealthMenuItem.Text = "Empty Health (Kill)";
            // 
            // fillHealthMenuItem
            // 
            this.fillHealthMenuItem.Name = "fillHealthMenuItem";
            this.fillHealthMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.K)));
            this.fillHealthMenuItem.Size = new System.Drawing.Size(260, 26);
            this.fillHealthMenuItem.Text = "Fill Health";
            // 
            // emptyWeaponMenuItem
            // 
            this.emptyWeaponMenuItem.Name = "emptyWeaponMenuItem";
            this.emptyWeaponMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.emptyWeaponMenuItem.Size = new System.Drawing.Size(260, 26);
            this.emptyWeaponMenuItem.Text = "Empty Weapon";
            // 
            // fillWeaponMenuIem
            // 
            this.fillWeaponMenuIem.Name = "fillWeaponMenuIem";
            this.fillWeaponMenuIem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.W)));
            this.fillWeaponMenuIem.Size = new System.Drawing.Size(260, 26);
            this.fillWeaponMenuIem.Text = "Fill Weapon";
            // 
            // layersToolStripMenuItem
            // 
            this.layersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backgroundToolStripMenuItem,
            this.sprites1ToolStripMenuItem,
            this.sprites2ToolStripMenuItem,
            this.sprites3ToolStripMenuItem,
            this.sprites4ToolStripMenuItem,
            this.foregroundToolStripMenuItem,
            this.toolStripSeparator5,
            this.activateAllToolStripMenuItem});
            this.layersToolStripMenuItem.Name = "layersToolStripMenuItem";
            this.layersToolStripMenuItem.Size = new System.Drawing.Size(289, 26);
            this.layersToolStripMenuItem.Text = "&Layers";
            // 
            // backgroundToolStripMenuItem
            // 
            this.backgroundToolStripMenuItem.Checked = true;
            this.backgroundToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.backgroundToolStripMenuItem.Name = "backgroundToolStripMenuItem";
            this.backgroundToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.backgroundToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.backgroundToolStripMenuItem.Text = "Background";
            // 
            // sprites1ToolStripMenuItem
            // 
            this.sprites1ToolStripMenuItem.Checked = true;
            this.sprites1ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sprites1ToolStripMenuItem.Name = "sprites1ToolStripMenuItem";
            this.sprites1ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.sprites1ToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.sprites1ToolStripMenuItem.Text = "Sprites 1";
            // 
            // sprites2ToolStripMenuItem
            // 
            this.sprites2ToolStripMenuItem.Checked = true;
            this.sprites2ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sprites2ToolStripMenuItem.Name = "sprites2ToolStripMenuItem";
            this.sprites2ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F9;
            this.sprites2ToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.sprites2ToolStripMenuItem.Text = "Sprites 2";
            // 
            // sprites3ToolStripMenuItem
            // 
            this.sprites3ToolStripMenuItem.Checked = true;
            this.sprites3ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sprites3ToolStripMenuItem.Name = "sprites3ToolStripMenuItem";
            this.sprites3ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.sprites3ToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.sprites3ToolStripMenuItem.Text = "Sprites 3";
            // 
            // sprites4ToolStripMenuItem
            // 
            this.sprites4ToolStripMenuItem.Checked = true;
            this.sprites4ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sprites4ToolStripMenuItem.Name = "sprites4ToolStripMenuItem";
            this.sprites4ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.sprites4ToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.sprites4ToolStripMenuItem.Text = "Sprites 4";
            // 
            // foregroundToolStripMenuItem
            // 
            this.foregroundToolStripMenuItem.Checked = true;
            this.foregroundToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.foregroundToolStripMenuItem.Name = "foregroundToolStripMenuItem";
            this.foregroundToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.foregroundToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.foregroundToolStripMenuItem.Text = "Foreground";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(200, 6);
            // 
            // activateAllToolStripMenuItem
            // 
            this.activateAllToolStripMenuItem.Name = "activateAllToolStripMenuItem";
            this.activateAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.activateAllToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.activateAllToolStripMenuItem.Text = "Restore All";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(286, 6);
            // 
            // framerateUpToolStripMenuItem
            // 
            this.framerateUpToolStripMenuItem.Name = "framerateUpToolStripMenuItem";
            this.framerateUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.framerateUpToolStripMenuItem.Size = new System.Drawing.Size(289, 26);
            this.framerateUpToolStripMenuItem.Text = "Framerate Up";
            this.framerateUpToolStripMenuItem.Click += new System.EventHandler(this.framerateUpToolStripMenuItem_Click);
            // 
            // framerateDownToolStripMenuItem
            // 
            this.framerateDownToolStripMenuItem.Name = "framerateDownToolStripMenuItem";
            this.framerateDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
            this.framerateDownToolStripMenuItem.Size = new System.Drawing.Size(289, 26);
            this.framerateDownToolStripMenuItem.Text = "Framerate Down";
            this.framerateDownToolStripMenuItem.Click += new System.EventHandler(this.framerateDownToolStripMenuItem_Click);
            // 
            // defaultFramerateToolStripMenuItem
            // 
            this.defaultFramerateToolStripMenuItem.Name = "defaultFramerateToolStripMenuItem";
            this.defaultFramerateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
            this.defaultFramerateToolStripMenuItem.Size = new System.Drawing.Size(289, 26);
            this.defaultFramerateToolStripMenuItem.Text = "Default Framerate";
            this.defaultFramerateToolStripMenuItem.Click += new System.EventHandler(this.defaultFramerateToolStripMenuItem_Click);
            // 
            // debugBar
            // 
            this.debugBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.debugBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fpsLabel,
            this.thinkLabel,
            this.entityLabel,
            this.fpsCapLabel});
            this.debugBar.Location = new System.Drawing.Point(0, 337);
            this.debugBar.Name = "debugBar";
            this.debugBar.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.debugBar.Size = new System.Drawing.Size(392, 25);
            this.debugBar.SizingGrip = false;
            this.debugBar.TabIndex = 2;
            this.debugBar.Text = "statusStrip1";
            // 
            // fpsLabel
            // 
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(17, 20);
            this.fpsLabel.Text = "0";
            // 
            // thinkLabel
            // 
            this.thinkLabel.Name = "thinkLabel";
            this.thinkLabel.Size = new System.Drawing.Size(17, 20);
            this.thinkLabel.Text = "0";
            // 
            // entityLabel
            // 
            this.entityLabel.Name = "entityLabel";
            this.entityLabel.Size = new System.Drawing.Size(17, 20);
            this.entityLabel.Text = "0";
            // 
            // fpsCapLabel
            // 
            this.fpsCapLabel.Name = "fpsCapLabel";
            this.fpsCapLabel.Size = new System.Drawing.Size(17, 20);
            this.fpsCapLabel.Text = "0";
            // 
            // xnaImage
            // 
            this.xnaImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xnaImage.Location = new System.Drawing.Point(0, 28);
            this.xnaImage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.xnaImage.Name = "xnaImage";
            this.xnaImage.NTSC = false;
            this.xnaImage.Size = new System.Drawing.Size(392, 309);
            this.xnaImage.TabIndex = 1;
            this.xnaImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.xnaImage_MouseClick);
            // 
            // openRecentToolStripMenuItem
            // 
            this.openRecentToolStripMenuItem.Name = "openRecentToolStripMenuItem";
            this.openRecentToolStripMenuItem.Size = new System.Drawing.Size(306, 26);
            this.openRecentToolStripMenuItem.Text = "Open Recent";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 362);
            this.Controls.Add(this.xnaImage);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.debugBar);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "Mega Man";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.debugBar.ResumeLayout(false);
            this.debugBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EngineGraphicsControl xnaImage;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem closeGameToolStripMenuItem;
        private StatusStrip debugBar;
        private ToolStripStatusLabel fpsLabel;
        private ToolStripStatusLabel thinkLabel;
        private ToolStripStatusLabel entityLabel;
        private ToolStripMenuItem quitToolStripMenuItem;
        private ToolStripMenuItem debugToolStripMenuItem;
        private ToolStripMenuItem debugBarToolStripMenuItem;
        private ToolStripMenuItem showHitboxesToolStripMenuItem;
        private ToolStripMenuItem invincibilityToolStripMenuItem;
        private ToolStripMenuItem gravityFlipToolStripMenuItem;
        private ToolStripMenuItem resetToolStripMenuItem;
        private ToolStripMenuItem inputToolStripMenuItem;
        private ToolStripMenuItem keyboardToolStripMenuItem;
        private ToolStripMenuItem layersToolStripMenuItem;
        private ToolStripMenuItem backgroundToolStripMenuItem;
        private ToolStripMenuItem sprites1ToolStripMenuItem;
        private ToolStripMenuItem sprites2ToolStripMenuItem;
        private ToolStripMenuItem sprites3ToolStripMenuItem;
        private ToolStripMenuItem sprites4ToolStripMenuItem;
        private ToolStripMenuItem foregroundToolStripMenuItem;
        private ToolStripMenuItem screenToolStripMenuItem;
        private ToolStripMenuItem screen1XMenu;
        private ToolStripMenuItem screen2XMenu;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem smoothedToolStripMenuItem;
        private ToolStripMenuItem pixellatedToolStripMenuItem;
        private ToolStripMenuItem screenNTSCMenu;
        private ToolStripMenuItem ntscComposite;
        private ToolStripMenuItem ntscSVideo;
        private ToolStripMenuItem ntscRGB;
        private ToolStripMenuItem ntscCustom;
        private ToolStripStatusLabel fpsCapLabel;
        private ToolStripMenuItem framerateUpToolStripMenuItem;
        private ToolStripMenuItem framerateDownToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem debugCheatMenu;
        private ToolStripMenuItem emptyHealthMenuItem;
        private ToolStripMenuItem fillHealthMenuItem;
        private ToolStripMenuItem emptyWeaponMenuItem;
        private ToolStripMenuItem fillWeaponMenuIem;
        private ToolStripMenuItem audioToolStripMenuItem;
        private ToolStripMenuItem musicMenuItem;
        private ToolStripMenuItem sfxMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem sq1MenuItem;
        private ToolStripMenuItem sq2MenuItem;
        private ToolStripMenuItem triMenuItem;
        private ToolStripMenuItem noiseMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem screenshotMenuItem;
        private ToolStripMenuItem hideMenuItem;
        private ToolStripMenuItem screen3XMenu;
        private ToolStripMenuItem screen4XMenu;
        private ToolStripMenuItem fullScreenToolStripMenuItem;
        private ToolStripMenuItem activateAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem defaultFramerateToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem pauseEngineToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem noDamageToolStripMenuItem;
        private ToolStripMenuItem increaseVolumeToolStripMenuItem;
        private ToolStripMenuItem decreaseVolumeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem defaultConfigToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem saveConfigurationsToolStripMenuItem;
        private ToolStripMenuItem autosaveToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem autoloadToolStripMenuItem;
        private ToolStripMenuItem loadConfigurationToolStripMenuItem;
        private ToolStripMenuItem deleteConfigurationsToolStripMenuItem;
        private ToolStripMenuItem openRecentToolStripMenuItem;
    }
}

