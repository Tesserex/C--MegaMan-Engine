using System;
using System.Windows.Forms;

namespace MegaMan.LevelEditor {
    public class HistoryForm : WeifenLuo.WinFormsUI.Docking.DockContent {
        private ListBox historyView;
    
        public HistoryForm() {
            InitializeComponent();
        }

        public void UpdateHistory(History history) {
            historyView.Items.Clear();

            for (int i = 0; i < history.stack.Count; i++) {
                if (history.currentAction == i)
                    historyView.Items.Add(" ->" + history.stack[i]);                    
                else
                    historyView.Items.Add(" * " + history.stack[i]);
            }
        }

        void InitializeComponent() {
            historyView = new ListBox();
            SuspendLayout();
            // 
            // historyView
            // 
            historyView.Dock = DockStyle.Fill;
            historyView.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            historyView.FormattingEnabled = true;
            historyView.ItemHeight = 18;
            historyView.Location = new System.Drawing.Point(0, 0);
            historyView.Name = "historyView";
            historyView.Size = new System.Drawing.Size(340, 238);
            historyView.TabIndex = 0;
            historyView.SelectedIndexChanged += historyView_SelectedIndexChanged;
            // 
            // HistoryForm
            // 
            ClientSize = new System.Drawing.Size(340, 241);
            Controls.Add(historyView);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "HistoryForm";
            Text = "History";
            Load += HistoryForm_Load;
            ResumeLayout(false);

        }

        private void historyView_SelectedIndexChanged(object sender, EventArgs e) {
            
        }

        private void HistoryForm_Load(object sender, EventArgs e) {
            
        }
    }
}
