using System;
using System.Windows.Forms;

namespace Mega_Man
{
    public partial class Keyboard : Form
    {
        private GameInput waitKey;
        private Label waitLabel;

        public Keyboard()
        {
            InitializeComponent();
            KeyPreview = true;
        }

        protected override void OnShown(EventArgs e)
        {
            upkeylabel.Text = GameInputKeys.Up.ToString();
            downkeylabel.Text = GameInputKeys.Down.ToString();
            leftkeylabel.Text = GameInputKeys.Left.ToString();
            rightkeylabel.Text = GameInputKeys.Right.ToString();
            jumpkeylabel.Text = GameInputKeys.Jump.ToString();
            shootkeylabel.Text = GameInputKeys.Shoot.ToString();
            startkeylabel.Text = GameInputKeys.Start.ToString();
            selectkeylabel.Text = GameInputKeys.Select.ToString();
            base.OnShown(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (waitLabel != null)
            {
                switch (waitKey)
                {
                    case GameInput.Up: GameInputKeys.Up = keyData; break;
                    case GameInput.Down: GameInputKeys.Down = keyData; break;
                    case GameInput.Left: GameInputKeys.Left = keyData; break;
                    case GameInput.Right: GameInputKeys.Right = keyData; break;
                    case GameInput.Jump: GameInputKeys.Jump = keyData; break;
                    case GameInput.Shoot: GameInputKeys.Shoot = keyData; break;
                    case GameInput.Start: GameInputKeys.Start = keyData; break;
                    case GameInput.Select: GameInputKeys.Select = keyData; break;
                }
                waitLabel.Text = keyData.ToString();
                waitLabel = null;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (waitLabel != null) return;
            upkeylabel.Text = "Waiting...";
            waitKey = GameInput.Up;
            waitLabel = upkeylabel;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (waitLabel != null) return;
            downkeylabel.Text = "Waiting...";
            waitKey = GameInput.Down;
            waitLabel = downkeylabel;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (waitLabel != null) return;
            leftkeylabel.Text = "Waiting...";
            waitKey = GameInput.Left;
            waitLabel = leftkeylabel;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (waitLabel != null) return;
            rightkeylabel.Text = "Waiting...";
            waitKey = GameInput.Right;
            waitLabel = rightkeylabel;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (waitLabel != null) return;
            jumpkeylabel.Text = "Waiting...";
            waitKey = GameInput.Jump;
            waitLabel = jumpkeylabel;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (waitLabel != null) return;
            shootkeylabel.Text = "Waiting...";
            waitKey = GameInput.Shoot;
            waitLabel = shootkeylabel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (waitLabel != null) return;
            startkeylabel.Text = "Waiting...";
            waitKey = GameInput.Start;
            waitLabel = startkeylabel;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (waitLabel != null) return;
            selectkeylabel.Text = "Waiting...";
            waitKey = GameInput.Select;
            waitLabel = selectkeylabel;
        }
    }
}
