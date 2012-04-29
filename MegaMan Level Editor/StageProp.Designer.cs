namespace MegaMan.LevelEditor {
    partial class StageProp
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
            this.tilesetChange = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tilesetField = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nameField = new System.Windows.Forms.TextBox();
            this.introField = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.loopField = new System.Windows.Forms.TextBox();
            this.introChange = new System.Windows.Forms.Button();
            this.loopChange = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tileset:";
            // 
            // tilesetChange
            // 
            this.tilesetChange.Location = new System.Drawing.Point(265, 30);
            this.tilesetChange.Name = "tilesetChange";
            this.tilesetChange.Size = new System.Drawing.Size(61, 20);
            this.tilesetChange.TabIndex = 2;
            this.tilesetChange.Text = "Browse";
            this.tilesetChange.UseVisualStyleBackColor = true;
            this.tilesetChange.Click += new System.EventHandler(this.tilesetChange_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(208, 111);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(54, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(268, 111);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(58, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tilesetField
            // 
            this.tilesetField.Location = new System.Drawing.Point(66, 30);
            this.tilesetField.Name = "tilesetField";
            this.tilesetField.Size = new System.Drawing.Size(195, 20);
            this.tilesetField.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(17, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Name:";
            // 
            // nameField
            // 
            this.nameField.Location = new System.Drawing.Point(66, 6);
            this.nameField.Name = "nameField";
            this.nameField.Size = new System.Drawing.Size(111, 20);
            this.nameField.TabIndex = 7;
            // 
            // introField
            // 
            this.introField.Location = new System.Drawing.Point(91, 56);
            this.introField.Name = "introField";
            this.introField.Size = new System.Drawing.Size(170, 20);
            this.introField.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Music Intro:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Music Loop:";
            // 
            // loopField
            // 
            this.loopField.Location = new System.Drawing.Point(91, 82);
            this.loopField.Name = "loopField";
            this.loopField.Size = new System.Drawing.Size(170, 20);
            this.loopField.TabIndex = 11;
            // 
            // introChange
            // 
            this.introChange.Location = new System.Drawing.Point(265, 56);
            this.introChange.Name = "introChange";
            this.introChange.Size = new System.Drawing.Size(61, 20);
            this.introChange.TabIndex = 12;
            this.introChange.Text = "Browse";
            this.introChange.UseVisualStyleBackColor = true;
            this.introChange.Click += new System.EventHandler(this.introChange_Click);
            // 
            // loopChange
            // 
            this.loopChange.Location = new System.Drawing.Point(265, 82);
            this.loopChange.Name = "loopChange";
            this.loopChange.Size = new System.Drawing.Size(61, 20);
            this.loopChange.TabIndex = 13;
            this.loopChange.Text = "Browse";
            this.loopChange.UseVisualStyleBackColor = true;
            this.loopChange.Click += new System.EventHandler(this.loopChange_Click);
            // 
            // StageProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 146);
            this.Controls.Add(this.loopChange);
            this.Controls.Add(this.introChange);
            this.Controls.Add(this.loopField);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.introField);
            this.Controls.Add(this.nameField);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tilesetField);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tilesetChange);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "StageProp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "StageProp";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button tilesetChange;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox tilesetField;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameField;
        private System.Windows.Forms.TextBox introField;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox loopField;
        private System.Windows.Forms.Button introChange;
        private System.Windows.Forms.Button loopChange;
    }
}