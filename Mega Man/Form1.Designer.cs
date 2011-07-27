namespace MegaMan.Engine
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.inputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screen1XMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.screen2XMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.screenNTSCMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ntscComposite = new System.Windows.Forms.ToolStripMenuItem();
            this.ntscSVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.ntscRGB = new System.Windows.Forms.ToolStripMenuItem();
            this.ntscCustom = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pixellatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smoothedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHitboxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invincibilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gravityFlipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sprites1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sprites2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sprites3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spries4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foregroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugBar = new System.Windows.Forms.StatusStrip();
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.thinkLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.entityLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.fpsCapLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.framerateUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.framerateDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xnaImage = new MegaMan.Engine.EngineGraphicsControl();
            this.menuStrip1.SuspendLayout();
            this.debugBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.inputToolStripMenuItem,
            this.screenToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(294, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.closeGameToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadToolStripMenuItem.Text = "Open Game";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // closeGameToolStripMenuItem
            // 
            this.closeGameToolStripMenuItem.Name = "closeGameToolStripMenuItem";
            this.closeGameToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.closeGameToolStripMenuItem.Text = "Close Game";
            this.closeGameToolStripMenuItem.Click += new System.EventHandler(this.closeGameToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // inputToolStripMenuItem
            // 
            this.inputToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keyboardToolStripMenuItem});
            this.inputToolStripMenuItem.Name = "inputToolStripMenuItem";
            this.inputToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.inputToolStripMenuItem.Text = "Input";
            // 
            // keyboardToolStripMenuItem
            // 
            this.keyboardToolStripMenuItem.Name = "keyboardToolStripMenuItem";
            this.keyboardToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.keyboardToolStripMenuItem.Text = "Keyboard";
            this.keyboardToolStripMenuItem.Click += new System.EventHandler(this.keyboardToolStripMenuItem_Click);
            // 
            // screenToolStripMenuItem
            // 
            this.screenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.screen1XMenu,
            this.screen2XMenu,
            this.screenNTSCMenu,
            this.toolStripSeparator1,
            this.pixellatedToolStripMenuItem,
            this.smoothedToolStripMenuItem});
            this.screenToolStripMenuItem.Name = "screenToolStripMenuItem";
            this.screenToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.screenToolStripMenuItem.Text = "Screen";
            // 
            // screen1XMenu
            // 
            this.screen1XMenu.Checked = true;
            this.screen1XMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.screen1XMenu.Name = "screen1XMenu";
            this.screen1XMenu.Size = new System.Drawing.Size(129, 22);
            this.screen1XMenu.Text = "1x";
            this.screen1XMenu.Click += new System.EventHandler(this.screen1XMenu_Click);
            // 
            // screen2XMenu
            // 
            this.screen2XMenu.Name = "screen2XMenu";
            this.screen2XMenu.Size = new System.Drawing.Size(129, 22);
            this.screen2XMenu.Text = "2x";
            this.screen2XMenu.Click += new System.EventHandler(this.screen2XMenu_Click);
            // 
            // screenNTSCMenu
            // 
            this.screenNTSCMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ntscComposite,
            this.ntscSVideo,
            this.ntscRGB,
            this.ntscCustom});
            this.screenNTSCMenu.Name = "screenNTSCMenu";
            this.screenNTSCMenu.Size = new System.Drawing.Size(129, 22);
            this.screenNTSCMenu.Text = "NTSC";
            this.screenNTSCMenu.Click += new System.EventHandler(this.screenNTSCMenu_Click);
            // 
            // ntscComposite
            // 
            this.ntscComposite.Checked = true;
            this.ntscComposite.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ntscComposite.Name = "ntscComposite";
            this.ntscComposite.Size = new System.Drawing.Size(132, 22);
            this.ntscComposite.Text = "Composite";
            this.ntscComposite.Click += new System.EventHandler(this.ntscComposite_Click);
            // 
            // ntscSVideo
            // 
            this.ntscSVideo.Name = "ntscSVideo";
            this.ntscSVideo.Size = new System.Drawing.Size(132, 22);
            this.ntscSVideo.Text = "S-Video";
            this.ntscSVideo.Click += new System.EventHandler(this.ntscSVideo_Click);
            // 
            // ntscRGB
            // 
            this.ntscRGB.Name = "ntscRGB";
            this.ntscRGB.Size = new System.Drawing.Size(132, 22);
            this.ntscRGB.Text = "RGB";
            this.ntscRGB.Click += new System.EventHandler(this.ntscRGB_Click);
            // 
            // ntscCustom
            // 
            this.ntscCustom.Name = "ntscCustom";
            this.ntscCustom.Size = new System.Drawing.Size(132, 22);
            this.ntscCustom.Text = "Custom ...";
            this.ntscCustom.Click += new System.EventHandler(this.ntscCustom_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(126, 6);
            // 
            // pixellatedToolStripMenuItem
            // 
            this.pixellatedToolStripMenuItem.Checked = true;
            this.pixellatedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pixellatedToolStripMenuItem.Name = "pixellatedToolStripMenuItem";
            this.pixellatedToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.pixellatedToolStripMenuItem.Text = "Pixellated";
            this.pixellatedToolStripMenuItem.Click += new System.EventHandler(this.pixellatedToolStripMenuItem_Click);
            // 
            // smoothedToolStripMenuItem
            // 
            this.smoothedToolStripMenuItem.Name = "smoothedToolStripMenuItem";
            this.smoothedToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.smoothedToolStripMenuItem.Text = "Smoothed";
            this.smoothedToolStripMenuItem.Click += new System.EventHandler(this.smoothedToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugBarToolStripMenuItem,
            this.showHitboxesToolStripMenuItem,
            this.invincibilityToolStripMenuItem,
            this.gravityFlipToolStripMenuItem,
            this.layersToolStripMenuItem,
            this.framerateUpToolStripMenuItem,
            this.framerateDownToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // debugBarToolStripMenuItem
            // 
            this.debugBarToolStripMenuItem.Name = "debugBarToolStripMenuItem";
            this.debugBarToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.debugBarToolStripMenuItem.Text = "Debug Bar";
            this.debugBarToolStripMenuItem.Click += new System.EventHandler(this.debugBarToolStripMenuItem_Click);
            // 
            // showHitboxesToolStripMenuItem
            // 
            this.showHitboxesToolStripMenuItem.Name = "showHitboxesToolStripMenuItem";
            this.showHitboxesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.showHitboxesToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.showHitboxesToolStripMenuItem.Text = "Show Hitboxes";
            this.showHitboxesToolStripMenuItem.Click += new System.EventHandler(this.showHitboxesToolStripMenuItem_Click);
            // 
            // invincibilityToolStripMenuItem
            // 
            this.invincibilityToolStripMenuItem.Name = "invincibilityToolStripMenuItem";
            this.invincibilityToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.invincibilityToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.invincibilityToolStripMenuItem.Text = "Invincibility";
            this.invincibilityToolStripMenuItem.Click += new System.EventHandler(this.invincibilityToolStripMenuItem_Click);
            // 
            // gravityFlipToolStripMenuItem
            // 
            this.gravityFlipToolStripMenuItem.Name = "gravityFlipToolStripMenuItem";
            this.gravityFlipToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gravityFlipToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.gravityFlipToolStripMenuItem.Text = "Gravity Flip";
            this.gravityFlipToolStripMenuItem.Click += new System.EventHandler(this.gravityFlipToolStripMenuItem_Click);
            // 
            // layersToolStripMenuItem
            // 
            this.layersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backgroundToolStripMenuItem,
            this.sprites1ToolStripMenuItem,
            this.sprites2ToolStripMenuItem,
            this.sprites3ToolStripMenuItem,
            this.spries4ToolStripMenuItem,
            this.foregroundToolStripMenuItem});
            this.layersToolStripMenuItem.Name = "layersToolStripMenuItem";
            this.layersToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.layersToolStripMenuItem.Text = "Layers";
            // 
            // backgroundToolStripMenuItem
            // 
            this.backgroundToolStripMenuItem.Checked = true;
            this.backgroundToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.backgroundToolStripMenuItem.Name = "backgroundToolStripMenuItem";
            this.backgroundToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.backgroundToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.backgroundToolStripMenuItem.Text = "Background";
            this.backgroundToolStripMenuItem.Click += new System.EventHandler(this.backgroundToolStripMenuItem_Click);
            // 
            // sprites1ToolStripMenuItem
            // 
            this.sprites1ToolStripMenuItem.Checked = true;
            this.sprites1ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sprites1ToolStripMenuItem.Name = "sprites1ToolStripMenuItem";
            this.sprites1ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.sprites1ToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.sprites1ToolStripMenuItem.Text = "Sprites 1";
            this.sprites1ToolStripMenuItem.Click += new System.EventHandler(this.sprites1ToolStripMenuItem_Click);
            // 
            // sprites2ToolStripMenuItem
            // 
            this.sprites2ToolStripMenuItem.Checked = true;
            this.sprites2ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sprites2ToolStripMenuItem.Name = "sprites2ToolStripMenuItem";
            this.sprites2ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F9;
            this.sprites2ToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.sprites2ToolStripMenuItem.Text = "Sprites 2";
            this.sprites2ToolStripMenuItem.Click += new System.EventHandler(this.sprites2ToolStripMenuItem_Click);
            // 
            // sprites3ToolStripMenuItem
            // 
            this.sprites3ToolStripMenuItem.Checked = true;
            this.sprites3ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sprites3ToolStripMenuItem.Name = "sprites3ToolStripMenuItem";
            this.sprites3ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.sprites3ToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.sprites3ToolStripMenuItem.Text = "Sprites 3";
            this.sprites3ToolStripMenuItem.Click += new System.EventHandler(this.sprites3ToolStripMenuItem_Click);
            // 
            // spries4ToolStripMenuItem
            // 
            this.spries4ToolStripMenuItem.Checked = true;
            this.spries4ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.spries4ToolStripMenuItem.Name = "spries4ToolStripMenuItem";
            this.spries4ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.spries4ToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.spries4ToolStripMenuItem.Text = "Sprites 4";
            this.spries4ToolStripMenuItem.Click += new System.EventHandler(this.sprites4ToolStripMenuItem_Click);
            // 
            // foregroundToolStripMenuItem
            // 
            this.foregroundToolStripMenuItem.Checked = true;
            this.foregroundToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.foregroundToolStripMenuItem.Name = "foregroundToolStripMenuItem";
            this.foregroundToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.foregroundToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.foregroundToolStripMenuItem.Text = "Foreground";
            this.foregroundToolStripMenuItem.Click += new System.EventHandler(this.foregroundToolStripMenuItem_Click);
            // 
            // debugBar
            // 
            this.debugBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fpsLabel,
            this.thinkLabel,
            this.entityLabel,
            this.fpsCapLabel});
            this.debugBar.Location = new System.Drawing.Point(0, 272);
            this.debugBar.Name = "debugBar";
            this.debugBar.Size = new System.Drawing.Size(294, 22);
            this.debugBar.TabIndex = 2;
            this.debugBar.Text = "statusStrip1";
            // 
            // fpsLabel
            // 
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(13, 17);
            this.fpsLabel.Text = "0";
            // 
            // thinkLabel
            // 
            this.thinkLabel.Name = "thinkLabel";
            this.thinkLabel.Size = new System.Drawing.Size(13, 17);
            this.thinkLabel.Text = "0";
            // 
            // entityLabel
            // 
            this.entityLabel.Name = "entityLabel";
            this.entityLabel.Size = new System.Drawing.Size(13, 17);
            this.entityLabel.Text = "0";
            // 
            // fpsCapLabel
            // 
            this.fpsCapLabel.Name = "fpsCapLabel";
            this.fpsCapLabel.Size = new System.Drawing.Size(13, 17);
            this.fpsCapLabel.Text = "0";
            // 
            // framerateUpToolStripMenuItem
            // 
            this.framerateUpToolStripMenuItem.Name = "framerateUpToolStripMenuItem";
            this.framerateUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.framerateUpToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.framerateUpToolStripMenuItem.Text = "Framerate Up";
            this.framerateUpToolStripMenuItem.Click += new System.EventHandler(this.framerateUpToolStripMenuItem_Click);
            // 
            // framerateDownToolStripMenuItem
            // 
            this.framerateDownToolStripMenuItem.Name = "framerateDownToolStripMenuItem";
            this.framerateDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.F)));
            this.framerateDownToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.framerateDownToolStripMenuItem.Text = "Framerate Down";
            this.framerateDownToolStripMenuItem.Click += new System.EventHandler(this.framerateDownToolStripMenuItem_Click);
            // 
            // xnaImage
            // 
            this.xnaImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xnaImage.Location = new System.Drawing.Point(0, 24);
            this.xnaImage.Name = "xnaImage";
            this.xnaImage.NTSC = false;
            this.xnaImage.Size = new System.Drawing.Size(294, 248);
            this.xnaImage.TabIndex = 1;
            this.xnaImage.Click += new System.EventHandler(this.xnaImage_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 294);
            this.Controls.Add(this.xnaImage);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.debugBar);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Mega Man";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.debugBar.ResumeLayout(false);
            this.debugBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MegaMan.Engine.EngineGraphicsControl xnaImage;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeGameToolStripMenuItem;
        private System.Windows.Forms.StatusStrip debugBar;
        private System.Windows.Forms.ToolStripStatusLabel fpsLabel;
        private System.Windows.Forms.ToolStripStatusLabel thinkLabel;
        private System.Windows.Forms.ToolStripStatusLabel entityLabel;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showHitboxesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invincibilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gravityFlipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keyboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sprites1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sprites2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sprites3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spries4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem foregroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screen1XMenu;
        private System.Windows.Forms.ToolStripMenuItem screen2XMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem smoothedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pixellatedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenNTSCMenu;
        private System.Windows.Forms.ToolStripMenuItem ntscComposite;
        private System.Windows.Forms.ToolStripMenuItem ntscSVideo;
        private System.Windows.Forms.ToolStripMenuItem ntscRGB;
        private System.Windows.Forms.ToolStripMenuItem ntscCustom;
        private System.Windows.Forms.ToolStripStatusLabel fpsCapLabel;
        private System.Windows.Forms.ToolStripMenuItem framerateUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem framerateDownToolStripMenuItem;
    }
}

