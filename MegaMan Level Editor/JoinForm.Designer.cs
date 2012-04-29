namespace MegaMan.LevelEditor
{
    partial class JoinForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.joinType = new System.Windows.Forms.ComboBox();
            this.s1label = new System.Windows.Forms.Label();
            this.s2label = new System.Windows.Forms.Label();
            this.screenOne = new System.Windows.Forms.ComboBox();
            this.screenTwo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.offsetOne = new System.Windows.Forms.NumericUpDown();
            this.offsetTwo = new System.Windows.Forms.NumericUpDown();
            this.forward = new System.Windows.Forms.RadioButton();
            this.backward = new System.Windows.Forms.RadioButton();
            this.bidirectional = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.width = new System.Windows.Forms.NumericUpDown();
            this.okButton = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.offsetOne)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetTwo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.width)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Join Direction";
            // 
            // joinType
            // 
            this.joinType.FormattingEnabled = true;
            this.joinType.Items.AddRange(new object[] {
            "Left - Right",
            "Up - Down"});
            this.joinType.Location = new System.Drawing.Point(92, 9);
            this.joinType.Name = "joinType";
            this.joinType.Size = new System.Drawing.Size(82, 21);
            this.joinType.TabIndex = 1;
            this.joinType.SelectedIndexChanged += new System.EventHandler(this.joinType_SelectedIndexChanged);
            // 
            // s1label
            // 
            this.s1label.Location = new System.Drawing.Point(8, 45);
            this.s1label.Name = "s1label";
            this.s1label.Size = new System.Drawing.Size(78, 15);
            this.s1label.TabIndex = 2;
            this.s1label.Text = "Left Screen";
            this.s1label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // s2label
            // 
            this.s2label.Location = new System.Drawing.Point(8, 76);
            this.s2label.Name = "s2label";
            this.s2label.Size = new System.Drawing.Size(78, 14);
            this.s2label.TabIndex = 3;
            this.s2label.Text = "Right Screen";
            this.s2label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // screenOne
            // 
            this.screenOne.FormattingEnabled = true;
            this.screenOne.Location = new System.Drawing.Point(92, 42);
            this.screenOne.Name = "screenOne";
            this.screenOne.Size = new System.Drawing.Size(82, 21);
            this.screenOne.TabIndex = 4;
            // 
            // screenTwo
            // 
            this.screenTwo.FormattingEnabled = true;
            this.screenTwo.Location = new System.Drawing.Point(92, 74);
            this.screenTwo.Name = "screenTwo";
            this.screenTwo.Size = new System.Drawing.Size(82, 21);
            this.screenTwo.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(189, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Start Tile";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(189, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Start Tile";
            // 
            // offsetOne
            // 
            this.offsetOne.Location = new System.Drawing.Point(243, 42);
            this.offsetOne.Name = "offsetOne";
            this.offsetOne.Size = new System.Drawing.Size(37, 20);
            this.offsetOne.TabIndex = 8;
            // 
            // offsetTwo
            // 
            this.offsetTwo.Location = new System.Drawing.Point(244, 74);
            this.offsetTwo.Name = "offsetTwo";
            this.offsetTwo.Size = new System.Drawing.Size(35, 20);
            this.offsetTwo.TabIndex = 9;
            // 
            // forward
            // 
            this.forward.AutoSize = true;
            this.forward.Location = new System.Drawing.Point(12, 105);
            this.forward.Name = "forward";
            this.forward.Size = new System.Drawing.Size(97, 17);
            this.forward.TabIndex = 10;
            this.forward.TabStop = true;
            this.forward.Text = "Rightward Only";
            this.forward.UseVisualStyleBackColor = true;
            // 
            // backward
            // 
            this.backward.AutoSize = true;
            this.backward.Location = new System.Drawing.Point(115, 105);
            this.backward.Name = "backward";
            this.backward.Size = new System.Drawing.Size(90, 17);
            this.backward.TabIndex = 11;
            this.backward.TabStop = true;
            this.backward.Text = "Leftward Only";
            this.backward.UseVisualStyleBackColor = true;
            // 
            // bidirectional
            // 
            this.bidirectional.AutoSize = true;
            this.bidirectional.Checked = true;
            this.bidirectional.Location = new System.Drawing.Point(211, 105);
            this.bidirectional.Name = "bidirectional";
            this.bidirectional.Size = new System.Drawing.Size(82, 17);
            this.bidirectional.TabIndex = 12;
            this.bidirectional.TabStop = true;
            this.bidirectional.Text = "Bidirectional";
            this.bidirectional.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(203, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Width";
            // 
            // width
            // 
            this.width.Location = new System.Drawing.Point(243, 9);
            this.width.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.width.Name = "width";
            this.width.Size = new System.Drawing.Size(36, 20);
            this.width.TabIndex = 14;
            this.width.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(144, 139);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(61, 23);
            this.okButton.TabIndex = 15;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(218, 139);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 16;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label2.Location = new System.Drawing.Point(11, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(283, 1);
            this.label2.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(10, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(283, 1);
            this.label3.TabIndex = 18;
            // 
            // JoinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 174);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.width);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.bidirectional);
            this.Controls.Add(this.backward);
            this.Controls.Add(this.forward);
            this.Controls.Add(this.offsetTwo);
            this.Controls.Add(this.offsetOne);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.screenTwo);
            this.Controls.Add(this.screenOne);
            this.Controls.Add(this.s2label);
            this.Controls.Add(this.s1label);
            this.Controls.Add(this.joinType);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "JoinForm";
            this.Text = "Join Properties";
            ((System.ComponentModel.ISupportInitialize)(this.offsetOne)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetTwo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.width)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox joinType;
        private System.Windows.Forms.Label s1label;
        private System.Windows.Forms.Label s2label;
        private System.Windows.Forms.ComboBox screenOne;
        private System.Windows.Forms.ComboBox screenTwo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown offsetOne;
        private System.Windows.Forms.NumericUpDown offsetTwo;
        private System.Windows.Forms.RadioButton forward;
        private System.Windows.Forms.RadioButton backward;
        private System.Windows.Forms.RadioButton bidirectional;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown width;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}