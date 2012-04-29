namespace MegaMan.LevelEditor
{
    partial class BrushForm
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
            this.brushPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonNewBrush = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.splitter = new System.Windows.Forms.SplitContainer();
            this.brushPict = new System.Windows.Forms.PictureBox();
            this.resetButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.heightBox = new System.Windows.Forms.TextBox();
            this.widthBox = new System.Windows.Forms.TextBox();
            this.cancelBrush = new System.Windows.Forms.Button();
            this.saveBrush = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
            this.splitter.Panel1.SuspendLayout();
            this.splitter.Panel2.SuspendLayout();
            this.splitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.brushPict)).BeginInit();
            this.SuspendLayout();
            // 
            // brushPanel
            // 
            this.brushPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brushPanel.Location = new System.Drawing.Point(0, 0);
            this.brushPanel.Name = "brushPanel";
            this.brushPanel.Size = new System.Drawing.Size(242, 206);
            this.brushPanel.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonNewBrush,
            this.deleteButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(246, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonNewBrush
            // 
            this.buttonNewBrush.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNewBrush.Image = global::MegaMan.LevelEditor.Properties.Resources.add_brush;
            this.buttonNewBrush.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNewBrush.Name = "buttonNewBrush";
            this.buttonNewBrush.Size = new System.Drawing.Size(23, 22);
            this.buttonNewBrush.Text = "toolStripButton1";
            this.buttonNewBrush.ToolTipText = "New Brush";
            this.buttonNewBrush.Click += new System.EventHandler(this.buttonNewBrush_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteButton.Image = global::MegaMan.LevelEditor.Properties.Resources.Remove;
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(23, 22);
            this.deleteButton.Text = "toolStripButton2";
            this.deleteButton.ToolTipText = "Delete Brush";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // splitter
            // 
            this.splitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitter.Location = new System.Drawing.Point(0, 25);
            this.splitter.Name = "splitter";
            this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitter.Panel1
            // 
            this.splitter.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitter.Panel1.Controls.Add(this.saveBrush);
            this.splitter.Panel1.Controls.Add(this.cancelBrush);
            this.splitter.Panel1.Controls.Add(this.brushPict);
            this.splitter.Panel1.Controls.Add(this.resetButton);
            this.splitter.Panel1.Controls.Add(this.label2);
            this.splitter.Panel1.Controls.Add(this.label1);
            this.splitter.Panel1.Controls.Add(this.heightBox);
            this.splitter.Panel1.Controls.Add(this.widthBox);
            this.splitter.Panel1MinSize = 0;
            // 
            // splitter.Panel2
            // 
            this.splitter.Panel2.Controls.Add(this.brushPanel);
            this.splitter.Size = new System.Drawing.Size(246, 360);
            this.splitter.SplitterDistance = 148;
            this.splitter.SplitterWidth = 2;
            this.splitter.TabIndex = 4;
            // 
            // brushPict
            // 
            this.brushPict.Location = new System.Drawing.Point(84, 10);
            this.brushPict.Name = "brushPict";
            this.brushPict.Size = new System.Drawing.Size(86, 71);
            this.brushPict.TabIndex = 13;
            this.brushPict.TabStop = false;
            this.brushPict.MouseDown += new System.Windows.Forms.MouseEventHandler(this.brushPict_MouseDown);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(10, 57);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(59, 24);
            this.resetButton.TabIndex = 12;
            this.resetButton.Text = "Resize";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "H";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "W";
            // 
            // heightBox
            // 
            this.heightBox.Location = new System.Drawing.Point(32, 31);
            this.heightBox.Name = "heightBox";
            this.heightBox.Size = new System.Drawing.Size(37, 20);
            this.heightBox.TabIndex = 9;
            // 
            // widthBox
            // 
            this.widthBox.Location = new System.Drawing.Point(32, 4);
            this.widthBox.Name = "widthBox";
            this.widthBox.Size = new System.Drawing.Size(37, 20);
            this.widthBox.TabIndex = 8;
            // 
            // cancelBrush
            // 
            this.cancelBrush.Location = new System.Drawing.Point(10, 116);
            this.cancelBrush.Name = "cancelBrush";
            this.cancelBrush.Size = new System.Drawing.Size(59, 23);
            this.cancelBrush.TabIndex = 14;
            this.cancelBrush.Text = "Cancel";
            this.cancelBrush.UseVisualStyleBackColor = true;
            this.cancelBrush.Click += new System.EventHandler(this.CancelNewBrush);
            // 
            // saveBrush
            // 
            this.saveBrush.Location = new System.Drawing.Point(10, 87);
            this.saveBrush.Name = "saveBrush";
            this.saveBrush.Size = new System.Drawing.Size(59, 23);
            this.saveBrush.TabIndex = 15;
            this.saveBrush.Text = "Save";
            this.saveBrush.UseVisualStyleBackColor = true;
            this.saveBrush.Click += new System.EventHandler(this.SaveNewBrush);
            // 
            // BrushForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(246, 385);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "BrushForm";
            this.ShowInTaskbar = false;
            this.Text = "Brushes";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitter.Panel1.ResumeLayout(false);
            this.splitter.Panel1.PerformLayout();
            this.splitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
            this.splitter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.brushPict)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel brushPanel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonNewBrush;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.SplitContainer splitter;
        private System.Windows.Forms.PictureBox brushPict;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox heightBox;
        private System.Windows.Forms.TextBox widthBox;
        private System.Windows.Forms.Button saveBrush;
        private System.Windows.Forms.Button cancelBrush;
    }
}