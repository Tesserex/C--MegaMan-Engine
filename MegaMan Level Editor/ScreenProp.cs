using System;
using System.Windows.Forms;

namespace MegaMan.LevelEditor
{
    public partial class ScreenProp : Form
    {
        private readonly ScreenDocument screen;
        private readonly StageDocument stage;

        private readonly bool is_new;

        public static void CreateScreen(StageDocument stage)
        {
            new ScreenProp(stage).Show();
        }

        public static void EditScreen(ScreenDocument screen)
        {
            new ScreenProp(screen).Show();
        }

        // this constructor implies a new screen
        private ScreenProp(StageDocument stage)
        {
            InitializeComponent();
            is_new = true;
            textName.Text = "";
            this.stage = stage;
        }

        // this implies editing a screen
        private ScreenProp(ScreenDocument screen)
        {
            InitializeComponent();

            this.screen = screen;
            stage = screen.Stage;

            textName.Text = screen.Name;
            widthField.Value = screen.Width;
            heightField.Value = screen.Height;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (textName.Text == "")
            {
                MessageBox.Show("Screen must have a name.", "CME Level Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (is_new)
            {
                stage.AddScreen(textName.Text, (int)widthField.Value, (int)heightField.Value);
            }
            else
            {
                // Rename the screen
                screen.Name = textName.Text;
                screen.Resize((int)widthField.Value, (int)heightField.Value);
            }

            Close();
        }
    }
}
