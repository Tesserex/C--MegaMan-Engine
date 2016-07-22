using System;
using System.Windows.Forms;

namespace MegaMan.Engine
{
    public partial class Keyboard : Form
    {
        private GameInput waitKey;
        private Label waitLabel;
        private Button previousSelection = null;

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
                if (!keyData.HasFlag(Keys.Control) && !keyData.HasFlag(Keys.Alt))
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
                    return true;   // Needs to be here, so if a key picked like up, selected button must not be changed.
                }
                else
                {
                    string key = "";

                    if (keyData.HasFlag(Keys.Control)) key = "ctrl";
                    else if (keyData.HasFlag(Keys.Alt)) key = "Alt";

                    MessageBox.Show(this, "Key " + key + " is not allowed.", "Unhauthorized key", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (waitLabel != null)
            {
                previousSelection.Select();
                return;
            }

            previousSelection = button1;
            upkeylabel.Text = "Waiting...";
            waitKey = GameInput.Up;
            waitLabel = upkeylabel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (waitLabel != null)
            {
                previousSelection.Select();
                return;
            }

            previousSelection = button2;
            startkeylabel.Text = "Waiting...";
            waitKey = GameInput.Start;
            waitLabel = startkeylabel;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (waitLabel != null)
            {
                previousSelection.Select();
                return;
            }

            previousSelection = button3;
            selectkeylabel.Text = "Waiting...";
            waitKey = GameInput.Select;
            waitLabel = selectkeylabel;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (waitLabel != null)
            {
                previousSelection.Select();
                return;
            }

            previousSelection = button4;
            shootkeylabel.Text = "Waiting...";
            waitKey = GameInput.Shoot;
            waitLabel = shootkeylabel;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (waitLabel != null)
            {
                previousSelection.Select();
                return;
            }

            previousSelection = button5;
            rightkeylabel.Text = "Waiting...";
            waitKey = GameInput.Right;
            waitLabel = rightkeylabel;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (waitLabel != null)
            {
                previousSelection.Select();
                return;
            }

            previousSelection = button6;
            downkeylabel.Text = "Waiting...";
            waitKey = GameInput.Down;
            waitLabel = downkeylabel;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (waitLabel != null)
            {
                previousSelection.Select();
                return;
            }

            previousSelection = button7;
            jumpkeylabel.Text = "Waiting...";
            waitKey = GameInput.Jump;
            waitLabel = jumpkeylabel;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (waitLabel != null)
            {
                previousSelection.Select();
                return;
            }

            previousSelection = button8;
            leftkeylabel.Text = "Waiting...";
            waitKey = GameInput.Left;
            waitLabel = leftkeylabel;
        }
    }
}
