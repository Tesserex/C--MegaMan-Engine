namespace MegaMan.LevelEditor
{
    partial class SoundControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.typeCombo = new System.Windows.Forms.ComboBox();
            this.nsfPanel = new System.Windows.Forms.Panel();
            this.trackNumeric = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nameText = new System.Windows.Forms.TextBox();
            this.wavPanel = new System.Windows.Forms.Panel();
            this.browseButton = new System.Windows.Forms.Button();
            this.pathText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.priorityNumeric = new System.Windows.Forms.NumericUpDown();
            this.nsfPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackNumeric)).BeginInit();
            this.wavPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.priorityNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // typeCombo
            // 
            this.typeCombo.ForeColor = System.Drawing.SystemColors.WindowText;
            this.typeCombo.FormattingEnabled = true;
            this.typeCombo.Items.AddRange(new object[] {
            "NSF",
            "Wav"});
            this.typeCombo.Location = new System.Drawing.Point(6, 3);
            this.typeCombo.Name = "typeCombo";
            this.typeCombo.Size = new System.Drawing.Size(73, 21);
            this.typeCombo.TabIndex = 0;
            this.typeCombo.Text = "Type";
            this.typeCombo.SelectedIndexChanged += new System.EventHandler(this.typeCombo_SelectedIndexChanged);
            // 
            // nsfPanel
            // 
            this.nsfPanel.Controls.Add(this.trackNumeric);
            this.nsfPanel.Controls.Add(this.label1);
            this.nsfPanel.Location = new System.Drawing.Point(6, 29);
            this.nsfPanel.Name = "nsfPanel";
            this.nsfPanel.Size = new System.Drawing.Size(90, 27);
            this.nsfPanel.TabIndex = 1;
            this.nsfPanel.Visible = false;
            // 
            // trackNumeric
            // 
            this.trackNumeric.Location = new System.Drawing.Point(44, 3);
            this.trackNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.trackNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.trackNumeric.Name = "trackNumeric";
            this.trackNumeric.Size = new System.Drawing.Size(40, 20);
            this.trackNumeric.TabIndex = 1;
            this.trackNumeric.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.trackNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Track";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(96, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name";
            // 
            // nameText
            // 
            this.nameText.Location = new System.Drawing.Point(137, 4);
            this.nameText.Name = "nameText";
            this.nameText.Size = new System.Drawing.Size(76, 20);
            this.nameText.TabIndex = 3;
            // 
            // wavPanel
            // 
            this.wavPanel.Controls.Add(this.browseButton);
            this.wavPanel.Controls.Add(this.pathText);
            this.wavPanel.Controls.Add(this.label3);
            this.wavPanel.Location = new System.Drawing.Point(6, 30);
            this.wavPanel.Name = "wavPanel";
            this.wavPanel.Size = new System.Drawing.Size(207, 26);
            this.wavPanel.TabIndex = 4;
            this.wavPanel.Visible = false;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(145, 3);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(60, 20);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            // 
            // pathText
            // 
            this.pathText.Location = new System.Drawing.Point(39, 3);
            this.pathText.Name = "pathText";
            this.pathText.ReadOnly = true;
            this.pathText.Size = new System.Drawing.Size(100, 20);
            this.pathText.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Path";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Priority";
            // 
            // priorityNumeric
            // 
            this.priorityNumeric.Location = new System.Drawing.Point(50, 57);
            this.priorityNumeric.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.priorityNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.priorityNumeric.Name = "priorityNumeric";
            this.priorityNumeric.Size = new System.Drawing.Size(46, 20);
            this.priorityNumeric.TabIndex = 6;
            this.priorityNumeric.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.priorityNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // SoundControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.priorityNumeric);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.wavPanel);
            this.Controls.Add(this.nameText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nsfPanel);
            this.Controls.Add(this.typeCombo);
            this.Name = "SoundControl";
            this.Size = new System.Drawing.Size(218, 89);
            this.nsfPanel.ResumeLayout(false);
            this.nsfPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackNumeric)).EndInit();
            this.wavPanel.ResumeLayout(false);
            this.wavPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.priorityNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox typeCombo;
        private System.Windows.Forms.Panel nsfPanel;
        private System.Windows.Forms.NumericUpDown trackNumeric;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameText;
        private System.Windows.Forms.Panel wavPanel;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox pathText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown priorityNumeric;

    }
}
