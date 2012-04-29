using System;
using System.Windows.Forms;

namespace MegaMan.LevelEditor
{
    public partial class ProjectProperties : Form
    {
        private ProjectEditor editor;
        private int lastWidth = 256, lastHeight = 224;

        public ProjectProperties()
        {
            InitializeComponent();

            textDir.Text = Environment.CurrentDirectory;
        }

        public ProjectProperties(ProjectEditor project)
        {
            InitializeComponent();

            editor = project;
            Text = project.Name + " Properties";
            textName.Text = project.Name;
            textAuthor.Text = project.Author;
            textWidth.Text = project.ScreenWidth.ToString();
            textHeight.Text = project.ScreenHeight.ToString();

            panelLocation.Visible = false;
            Height -= panelLocation.Height;
        }

        private void text_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void textHeight_Leave(object sender, EventArgs e)
        {
            if (!int.TryParse(textHeight.Text, out lastHeight))
            {
                MessageBox.Show("Positive integer required for screen height.", "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textHeight.Text = lastHeight.ToString();
            }
        }

        private void textWidth_Leave(object sender, EventArgs e)
        {
            if (!int.TryParse(textWidth.Text, out lastWidth))
            {
                MessageBox.Show("Positive integer required for screen width.", "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textWidth.Text = lastWidth.ToString();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            int width, height;
            if (!int.TryParse(textWidth.Text, out width) || !int.TryParse(textHeight.Text, out height))
            {
                MessageBox.Show("Positive integers are required for screen size.", "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool gameNew = (editor == null);
            if (gameNew)
            {
                string baseDir = System.IO.Path.Combine(textDir.Text, textName.Text);
                if (System.IO.Directory.Exists(baseDir))
                {
                    MessageBox.Show(
                        String.Format("Could not create the project because a directory named {0} already exists at the specified location.", textName.Text),
                        "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    System.IO.Directory.CreateDirectory(baseDir);
                }
                catch
                {
                    MessageBox.Show(
                        "Could not create the project because the system was unable to create a directory at the specified location.",
                        "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                editor = ProjectEditor.CreateNew();
            }

            editor.Name = (textName.Text == "")? "Untitled" : textName.Text;
            editor.Author = textAuthor.Text;
            editor.ScreenWidth = lastWidth;
            editor.ScreenHeight = lastHeight;

            if (gameNew)
            {
                editor.Save();
                MainForm.Instance.projectForm.AddProject(editor);
            }
            Close();
        }

        private void textDir_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = System.IO.Path.IsPathRooted(textDir.Text);
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog {SelectedPath = textDir.Text, ShowNewFolderButton = true};
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textDir.Text = dialog.SelectedPath;
            }
        }
    }
}
