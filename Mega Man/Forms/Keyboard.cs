using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using MegaMan.Engine.Input;

namespace MegaMan.Engine
{
    public partial class Keyboard : Form
    {
        private GameInputs waitKey;
        private Label waitLabel;
        private Button previousSelection = null;

        public Keyboard()
        {
            InitializeComponent();
            KeyPreview = true;
        }

        protected override void OnShown(EventArgs e)
        {
            upkeylabel.Text = GetBindingDisplay(GameInputs.Up);
            downkeylabel.Text = GetBindingDisplay(GameInputs.Down);
            leftkeylabel.Text = GetBindingDisplay(GameInputs.Left);
            rightkeylabel.Text = GetBindingDisplay(GameInputs.Right);
            jumpkeylabel.Text = GetBindingDisplay(GameInputs.Jump);
            shootkeylabel.Text = GetBindingDisplay(GameInputs.Shoot);
            startkeylabel.Text = GetBindingDisplay(GameInputs.Start);
            selectkeylabel.Text = GetBindingDisplay(GameInputs.Select);

            DeviceManager.Instance.JoystickButtonPressed += JoystickButtonPressed;
            DeviceManager.Instance.JoystickAxisPressed += JoystickAxisPressed;
            DeviceManager.Instance.GamepadButtonPressed += GamepadButtonPressed;

            base.OnShown(e);
        }

        private string GetBindingDisplay(GameInputs input)
        {
            var binding = GameInput.GetBindings(input).FirstOrDefault();
            return binding != null ? binding.ToString() : "NONE";
        }

        /// <summary>
        /// Form isn't close, we just hide it and show it.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;    // If closing it, there will be a failure on call of show method.
            DeviceManager.Instance.JoystickButtonPressed -= JoystickButtonPressed;
            DeviceManager.Instance.JoystickAxisPressed -= JoystickAxisPressed;
            DeviceManager.Instance.GamepadButtonPressed -= GamepadButtonPressed;
            base.OnClosing(e);
            this.Hide();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (waitLabel != null)
            {
                if (!keyData.HasFlag(Keys.Control) && !keyData.HasFlag(Keys.Alt) && !keyData.HasFlag(Keys.Shift))
                {
                    var binding = new KeyboardInputBinding(waitKey, keyData);
                    GameInput.ClearBinding(waitKey);
                    GameInput.AddBinding(binding);
                    waitLabel.Text = binding.ToString();
                    waitLabel = null;
                    return true;   // Needs to be here, so if a key picked like up, selected button must not be changed.
                }
                else
                {
                    string key = "";

                    if (keyData.HasFlag(Keys.Control)) key = "ctrl";
                    else if (keyData.HasFlag(Keys.Alt)) key = "Alt";
                    else if (keyData.HasFlag(Keys.Shift)) key = "Shift";

                    MessageBox.Show(this, "Key " + key + " is not allowed.", "Unauthorized key", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void JoystickButtonPressed(object sender, JoystickButtonPressedEventArgs e)
        {
            if (waitLabel != null)
            {
                var binding = new JoystickInputBinding(waitKey, e.Button.DeviceGuid, e.Button.ButtonOffset);
                GameInput.ClearBinding(waitKey);
                GameInput.AddBinding(binding);
                waitLabel.Text = binding.ToString();
                waitLabel = null;
            }
        }

        private void JoystickAxisPressed(object sender, JoystickAxisPressedEventArgs e)
        {
            if (waitLabel != null)
            {
                var binding = new JoystickInputBinding(waitKey, e.Button.DeviceGuid, e.Button.ButtonOffset, e.Value);
                GameInput.ClearBinding(waitKey);
                GameInput.AddBinding(binding);
                waitLabel.Text = binding.ToString();
                waitLabel = null;
            }
        }

        private void GamepadButtonPressed(object sender, GamepadButtonPressedEventArgs e)
        {
            if (waitLabel != null)
            {
                var binding = new GamepadInputBinding(waitKey, e.Button);
                GameInput.ClearBinding(waitKey);
                GameInput.AddBinding(binding);
                waitLabel.Text = binding.ToString();
                waitLabel = null;
            }
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
            waitKey = GameInputs.Up;
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
            waitKey = GameInputs.Start;
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
            waitKey = GameInputs.Select;
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
            waitKey = GameInputs.Shoot;
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
            waitKey = GameInputs.Right;
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
            waitKey = GameInputs.Down;
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
            waitKey = GameInputs.Jump;
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
            waitKey = GameInputs.Left;
            waitLabel = leftkeylabel;
        }
    }
}
