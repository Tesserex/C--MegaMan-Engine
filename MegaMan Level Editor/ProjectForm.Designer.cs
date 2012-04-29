namespace MegaMan.LevelEditor {
    partial class ProjectForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.projectView = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonNewStage = new System.Windows.Forms.ToolStripButton();
            this.buttonNewScreen = new System.Windows.Forms.ToolStripButton();
            this.buttonProperties = new System.Windows.Forms.ToolStripButton();
            this.buttonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // projectView
            // 
            this.projectView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectView.Location = new System.Drawing.Point(0, 25);
            this.projectView.Name = "projectView";
            this.projectView.Size = new System.Drawing.Size(266, 207);
            this.projectView.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonNewStage,
            this.buttonNewScreen,
            this.buttonProperties,
            this.toolStripSeparator1,
            this.buttonDelete});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(266, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonNewStage
            // 
            this.buttonNewStage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNewStage.Image = global::MegaMan.LevelEditor.Properties.Resources.newstage;
            this.buttonNewStage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNewStage.Name = "buttonNewStage";
            this.buttonNewStage.Size = new System.Drawing.Size(23, 22);
            this.buttonNewStage.Text = "New Stage";
            this.buttonNewStage.Click += new System.EventHandler(this.buttonNewStage_Click);
            // 
            // buttonNewScreen
            // 
            this.buttonNewScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNewScreen.Image = global::MegaMan.LevelEditor.Properties.Resources.newscreen;
            this.buttonNewScreen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNewScreen.Name = "buttonNewScreen";
            this.buttonNewScreen.Size = new System.Drawing.Size(23, 22);
            this.buttonNewScreen.Text = "New Screen";
            this.buttonNewScreen.Click += new System.EventHandler(this.buttonNewScreen_Click);
            // 
            // buttonProperties
            // 
            this.buttonProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonProperties.Image = global::MegaMan.LevelEditor.Properties.Resources.Settings;
            this.buttonProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.Size = new System.Drawing.Size(23, 22);
            this.buttonProperties.Text = "Properties";
            this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDelete.Image = global::MegaMan.LevelEditor.Properties.Resources.Remove;
            this.buttonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(23, 22);
            this.buttonDelete.Text = "Delete";
            // 
            // ProjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 232);
            this.Controls.Add(this.projectView);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ProjectForm";
            this.Text = "Project";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TreeView projectView;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonNewStage;
        private System.Windows.Forms.ToolStripButton buttonNewScreen;
        private System.Windows.Forms.ToolStripButton buttonProperties;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonDelete;
    }
}