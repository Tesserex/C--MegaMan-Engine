namespace MegaMan.Engine.Forms
{
    partial class DeleteConfigs
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
            this.lblInstruction = new System.Windows.Forms.Label();
            this.cbxConfigToDelete = new System.Windows.Forms.ComboBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblInstruction
            // 
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.Location = new System.Drawing.Point(9, 15);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(102, 13);
            this.lblInstruction.TabIndex = 9;
            this.lblInstruction.Text = "Game to delete from";
            // 
            // cbxConfigToDelete
            // 
            this.cbxConfigToDelete.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxConfigToDelete.FormattingEnabled = true;
            this.cbxConfigToDelete.Location = new System.Drawing.Point(12, 31);
            this.cbxConfigToDelete.Name = "cbxConfigToDelete";
            this.cbxConfigToDelete.Size = new System.Drawing.Size(351, 21);
            this.cbxConfigToDelete.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(288, 59);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(12, 58);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.Location = new System.Drawing.Point(12, 87);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(351, 23);
            this.btnDeleteAll.TabIndex = 4;
            this.btnDeleteAll.Text = "Delete All";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // DeleteConfigs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 115);
            this.Controls.Add(this.btnDeleteAll);
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.cbxConfigToDelete);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDelete);
            this.Name = "DeleteConfigs";
            this.Text = "DeleteConfigs";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.ComboBox cbxConfigToDelete;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnDeleteAll;
    }
}