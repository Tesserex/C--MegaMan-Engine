namespace MegaMan.LevelEditor
{
    partial class SpriteEditor
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
            this.imagePanel = new System.Windows.Forms.Panel();
            this.sourceImage = new System.Windows.Forms.PictureBox();
            this.groupSheet = new System.Windows.Forms.GroupBox();
            this.checkSnap = new System.Windows.Forms.CheckBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textWidth = new System.Windows.Forms.TextBox();
            this.textHeight = new System.Windows.Forms.TextBox();
            this.buttonRefreshSize = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupFrame = new System.Windows.Forms.GroupBox();
            this.buttonAddFrame = new System.Windows.Forms.Button();
            this.frameTotalLabel = new System.Windows.Forms.Label();
            this.duration = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.currentFrame = new System.Windows.Forms.NumericUpDown();
            this.spritePreview = new System.Windows.Forms.PictureBox();
            this.radioWhite = new System.Windows.Forms.RadioButton();
            this.radioBlack = new System.Windows.Forms.RadioButton();
            this.buttonOk = new System.Windows.Forms.Button();
            this.imagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sourceImage)).BeginInit();
            this.groupSheet.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupFrame.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.duration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spritePreview)).BeginInit();
            this.SuspendLayout();
            // 
            // imagePanel
            // 
            this.imagePanel.AutoScroll = true;
            this.imagePanel.Controls.Add(this.sourceImage);
            this.imagePanel.Location = new System.Drawing.Point(6, 46);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(192, 192);
            this.imagePanel.TabIndex = 0;
            // 
            // sourceImage
            // 
            this.sourceImage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.sourceImage.Location = new System.Drawing.Point(0, 0);
            this.sourceImage.Name = "sourceImage";
            this.sourceImage.Size = new System.Drawing.Size(189, 192);
            this.sourceImage.TabIndex = 0;
            this.sourceImage.TabStop = false;
            this.sourceImage.MouseLeave += new System.EventHandler(this.sourceImage_MouseLeave);
            this.sourceImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.sourceImage_MouseMove);
            this.sourceImage.Click += new System.EventHandler(this.sourceImage_Click);
            this.sourceImage.MouseEnter += new System.EventHandler(this.sourceImage_MouseEnter);
            // 
            // groupSheet
            // 
            this.groupSheet.Controls.Add(this.checkSnap);
            this.groupSheet.Controls.Add(this.browseButton);
            this.groupSheet.Controls.Add(this.imagePanel);
            this.groupSheet.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupSheet.Location = new System.Drawing.Point(5, 5);
            this.groupSheet.Margin = new System.Windows.Forms.Padding(10);
            this.groupSheet.Name = "groupSheet";
            this.groupSheet.Size = new System.Drawing.Size(205, 246);
            this.groupSheet.TabIndex = 1;
            this.groupSheet.TabStop = false;
            this.groupSheet.Text = "Tile Sheet";
            // 
            // checkSnap
            // 
            this.checkSnap.AutoSize = true;
            this.checkSnap.Checked = true;
            this.checkSnap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkSnap.Location = new System.Drawing.Point(116, 24);
            this.checkSnap.Name = "checkSnap";
            this.checkSnap.Size = new System.Drawing.Size(84, 17);
            this.checkSnap.TabIndex = 3;
            this.checkSnap.Text = "Snap Cursor";
            this.checkSnap.UseVisualStyleBackColor = true;
            this.checkSnap.CheckedChanged += new System.EventHandler(this.checkSnap_CheckedChanged);
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(6, 19);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(100, 21);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse for File...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Width:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Height:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(100, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "pixels";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(100, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "pixels";
            // 
            // textWidth
            // 
            this.textWidth.Location = new System.Drawing.Point(50, 18);
            this.textWidth.Name = "textWidth";
            this.textWidth.Size = new System.Drawing.Size(44, 20);
            this.textWidth.TabIndex = 6;
            // 
            // textHeight
            // 
            this.textHeight.Location = new System.Drawing.Point(50, 44);
            this.textHeight.Name = "textHeight";
            this.textHeight.Size = new System.Drawing.Size(44, 20);
            this.textHeight.TabIndex = 7;
            // 
            // buttonRefreshSize
            // 
            this.buttonRefreshSize.Location = new System.Drawing.Point(159, 41);
            this.buttonRefreshSize.Name = "buttonRefreshSize";
            this.buttonRefreshSize.Size = new System.Drawing.Size(80, 23);
            this.buttonRefreshSize.TabIndex = 8;
            this.buttonRefreshSize.Text = "Refresh Size";
            this.buttonRefreshSize.UseVisualStyleBackColor = true;
            this.buttonRefreshSize.Click += new System.EventHandler(this.buttonRefreshSize_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.buttonRefreshSize);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textHeight);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textWidth);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(223, 7);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(245, 70);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sprite Size";
            // 
            // groupFrame
            // 
            this.groupFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupFrame.Controls.Add(this.buttonAddFrame);
            this.groupFrame.Controls.Add(this.frameTotalLabel);
            this.groupFrame.Controls.Add(this.duration);
            this.groupFrame.Controls.Add(this.label6);
            this.groupFrame.Controls.Add(this.label5);
            this.groupFrame.Controls.Add(this.currentFrame);
            this.groupFrame.Location = new System.Drawing.Point(224, 85);
            this.groupFrame.Name = "groupFrame";
            this.groupFrame.Size = new System.Drawing.Size(245, 73);
            this.groupFrame.TabIndex = 10;
            this.groupFrame.TabStop = false;
            this.groupFrame.Text = "Frames";
            // 
            // buttonAddFrame
            // 
            this.buttonAddFrame.Location = new System.Drawing.Point(164, 44);
            this.buttonAddFrame.Name = "buttonAddFrame";
            this.buttonAddFrame.Size = new System.Drawing.Size(75, 23);
            this.buttonAddFrame.TabIndex = 5;
            this.buttonAddFrame.Text = "Add Frame";
            this.buttonAddFrame.UseVisualStyleBackColor = true;
            this.buttonAddFrame.Click += new System.EventHandler(this.buttonAddFrame_Click);
            // 
            // frameTotalLabel
            // 
            this.frameTotalLabel.AutoSize = true;
            this.frameTotalLabel.Location = new System.Drawing.Point(144, 23);
            this.frameTotalLabel.Name = "frameTotalLabel";
            this.frameTotalLabel.Size = new System.Drawing.Size(25, 13);
            this.frameTotalLabel.TabIndex = 4;
            this.frameTotalLabel.Text = "of 0";
            // 
            // duration
            // 
            this.duration.Location = new System.Drawing.Point(97, 46);
            this.duration.Name = "duration";
            this.duration.Size = new System.Drawing.Size(41, 20);
            this.duration.TabIndex = 3;
            this.duration.ValueChanged += new System.EventHandler(this.duration_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Frame Duration:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Current Frame:";
            // 
            // currentFrame
            // 
            this.currentFrame.Location = new System.Drawing.Point(97, 21);
            this.currentFrame.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.currentFrame.Name = "currentFrame";
            this.currentFrame.Size = new System.Drawing.Size(41, 20);
            this.currentFrame.TabIndex = 0;
            this.currentFrame.ValueChanged += new System.EventHandler(this.currentFrame_ValueChanged);
            // 
            // spritePreview
            // 
            this.spritePreview.BackColor = System.Drawing.Color.White;
            this.spritePreview.Location = new System.Drawing.Point(223, 165);
            this.spritePreview.Name = "spritePreview";
            this.spritePreview.Size = new System.Drawing.Size(91, 83);
            this.spritePreview.TabIndex = 0;
            this.spritePreview.TabStop = false;
            // 
            // radioWhite
            // 
            this.radioWhite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioWhite.AutoSize = true;
            this.radioWhite.Checked = true;
            this.radioWhite.Location = new System.Drawing.Point(325, 170);
            this.radioWhite.Name = "radioWhite";
            this.radioWhite.Size = new System.Drawing.Size(114, 17);
            this.radioWhite.TabIndex = 11;
            this.radioWhite.TabStop = true;
            this.radioWhite.Text = "White Background";
            this.radioWhite.UseVisualStyleBackColor = true;
            this.radioWhite.CheckedChanged += new System.EventHandler(this.radioWhite_CheckedChanged);
            // 
            // radioBlack
            // 
            this.radioBlack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioBlack.AutoSize = true;
            this.radioBlack.Location = new System.Drawing.Point(325, 193);
            this.radioBlack.Name = "radioBlack";
            this.radioBlack.Size = new System.Drawing.Size(113, 17);
            this.radioBlack.TabIndex = 12;
            this.radioBlack.TabStop = true;
            this.radioBlack.Text = "Black Background";
            this.radioBlack.UseVisualStyleBackColor = true;
            this.radioBlack.CheckedChanged += new System.EventHandler(this.radioBlack_CheckedChanged);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(395, 222);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 13;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // SpriteEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 256);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.radioBlack);
            this.Controls.Add(this.radioWhite);
            this.Controls.Add(this.spritePreview);
            this.Controls.Add(this.groupFrame);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupSheet);
            this.Name = "SpriteEditor";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "SpriteEditor";
            this.imagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sourceImage)).EndInit();
            this.groupSheet.ResumeLayout(false);
            this.groupSheet.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupFrame.ResumeLayout(false);
            this.groupFrame.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.duration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spritePreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel imagePanel;
        private System.Windows.Forms.PictureBox sourceImage;
        private System.Windows.Forms.GroupBox groupSheet;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textWidth;
        private System.Windows.Forms.TextBox textHeight;
        private System.Windows.Forms.Button buttonRefreshSize;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupFrame;
        private System.Windows.Forms.NumericUpDown currentFrame;
        private System.Windows.Forms.NumericUpDown duration;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonAddFrame;
        private System.Windows.Forms.Label frameTotalLabel;
        private System.Windows.Forms.PictureBox spritePreview;
        private System.Windows.Forms.RadioButton radioWhite;
        private System.Windows.Forms.RadioButton radioBlack;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.CheckBox checkSnap;
    }
}