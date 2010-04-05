namespace Mega_Man
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
            this.screenImage = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHitboxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invincibilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gravityFlipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugBar = new System.Windows.Forms.StatusStrip();
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.thinkLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.entityLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.inputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.screenImage)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.debugBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // screenImage
            // 
            this.screenImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenImage.Location = new System.Drawing.Point(0, 24);
            this.screenImage.Margin = new System.Windows.Forms.Padding(0);
            this.screenImage.Name = "screenImage";
            this.screenImage.Size = new System.Drawing.Size(292, 248);
            this.screenImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.screenImage.TabIndex = 0;
            this.screenImage.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.inputToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(292, 24);
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
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.loadToolStripMenuItem.Text = "Open Game";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // closeGameToolStripMenuItem
            // 
            this.closeGameToolStripMenuItem.Name = "closeGameToolStripMenuItem";
            this.closeGameToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.closeGameToolStripMenuItem.Text = "Close Game";
            this.closeGameToolStripMenuItem.Click += new System.EventHandler(this.closeGameToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugBarToolStripMenuItem,
            this.showHitboxesToolStripMenuItem,
            this.invincibilityToolStripMenuItem,
            this.gravityFlipToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // debugBarToolStripMenuItem
            // 
            this.debugBarToolStripMenuItem.Name = "debugBarToolStripMenuItem";
            this.debugBarToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.debugBarToolStripMenuItem.Text = "Debug Bar";
            this.debugBarToolStripMenuItem.Click += new System.EventHandler(this.debugBarToolStripMenuItem_Click);
            // 
            // showHitboxesToolStripMenuItem
            // 
            this.showHitboxesToolStripMenuItem.Name = "showHitboxesToolStripMenuItem";
            this.showHitboxesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.showHitboxesToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.showHitboxesToolStripMenuItem.Text = "Show Hitboxes";
            this.showHitboxesToolStripMenuItem.Click += new System.EventHandler(this.showHitboxesToolStripMenuItem_Click);
            // 
            // invincibilityToolStripMenuItem
            // 
            this.invincibilityToolStripMenuItem.Name = "invincibilityToolStripMenuItem";
            this.invincibilityToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.invincibilityToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.invincibilityToolStripMenuItem.Text = "Invincibility";
            this.invincibilityToolStripMenuItem.Click += new System.EventHandler(this.invincibilityToolStripMenuItem_Click);
            // 
            // gravityFlipToolStripMenuItem
            // 
            this.gravityFlipToolStripMenuItem.Name = "gravityFlipToolStripMenuItem";
            this.gravityFlipToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gravityFlipToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.gravityFlipToolStripMenuItem.Text = "Gravity Flip";
            this.gravityFlipToolStripMenuItem.Click += new System.EventHandler(this.gravityFlipToolStripMenuItem_Click);
            // 
            // debugBar
            // 
            this.debugBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fpsLabel,
            this.thinkLabel,
            this.entityLabel});
            this.debugBar.Location = new System.Drawing.Point(0, 272);
            this.debugBar.Name = "debugBar";
            this.debugBar.Size = new System.Drawing.Size(292, 22);
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
            this.keyboardToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.keyboardToolStripMenuItem.Text = "Keyboard";
            this.keyboardToolStripMenuItem.Click += new System.EventHandler(this.keyboardToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 294);
            this.Controls.Add(this.screenImage);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.debugBar);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Mega Man";
            ((System.ComponentModel.ISupportInitialize)(this.screenImage)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.debugBar.ResumeLayout(false);
            this.debugBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox screenImage;
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
    }
}

