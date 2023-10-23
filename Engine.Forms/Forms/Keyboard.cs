using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using MegaMan.Engine.Forms;
using MegaMan.Engine.Input;

namespace MegaMan.Engine
{
    public partial class Keyboard : Form
    {
        private GameInputs? waitKey;

        public Keyboard()
        {
            InitializeComponent();
            KeyPreview = true;
        }

        protected override void OnShown(EventArgs e)
        {
            SetLabels(GameInputs.Up);
            SetLabels(GameInputs.Down);
            SetLabels(GameInputs.Left);
            SetLabels(GameInputs.Right);
            SetLabels(GameInputs.Jump);
            SetLabels(GameInputs.Shoot);
            SetLabels(GameInputs.Start);
            SetLabels(GameInputs.Select);

            switch (GameInput.ActiveType)
            {
                case InputTypes.Keyboard:
                    btnKeyboard.Checked = true;
                    break;
                case InputTypes.Gamepad:
                    btnGamepad.Checked = true;
                    break;
                case InputTypes.Joystick:
                    btnJoystick.Checked = true;
                    break;
            }

            DeviceManager.Instance.JoystickButtonPressed += JoystickButtonPressed;
            DeviceManager.Instance.JoystickAxisPressed += JoystickAxisPressed;
            DeviceManager.Instance.GamepadButtonPressed += GamepadButtonPressed;

            base.OnShown(e);
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
            Hide();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (waitKey != null)
            {
                if (!keyData.HasFlag(Keys.Control) && !keyData.HasFlag(Keys.Alt) && !keyData.HasFlag(Keys.Shift))
                {
                    var binding = new KeyboardInputBinding(waitKey.Value, (Microsoft.Xna.Framework.Input.Keys)(int)keyData, k => Program.KeyDown(keyData));
                    GameInput.AddBinding(binding);
                    SetLabels(waitKey.Value);
                    waitKey = null;
                    return true;   // Needs to be here, so if a key picked like up, selected button must not be changed.
                }

                var key = "";

                if (keyData.HasFlag(Keys.Control)) key = "ctrl";
                else if (keyData.HasFlag(Keys.Alt)) key = "Alt";
                else if (keyData.HasFlag(Keys.Shift)) key = "Shift";

                MessageBox.Show(this, "Key " + key + " is not allowed.", "Unauthorized key", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void JoystickButtonPressed(object? sender, JoystickButtonPressedEventArgs e)
        {
            if (waitKey != null)
            {
                var binding = new JoystickInputBinding(waitKey.Value, e.Button.DeviceGuid, e.Button.ButtonOffset);
                GameInput.AddBinding(binding);
                SetLabels(waitKey.Value);
                waitKey = null;
            }
        }

        private void JoystickAxisPressed(object? sender, JoystickAxisPressedEventArgs e)
        {
            if (waitKey != null)
            {
                var binding = new JoystickInputBinding(waitKey.Value, e.Button.DeviceGuid, e.Button.ButtonOffset, e.Value);
                GameInput.AddBinding(binding);
                SetLabels(waitKey.Value);
                waitKey = null;
            }
        }

        private void GamepadButtonPressed(object? sender, GamepadButtonPressedEventArgs e)
        {
            if (waitKey != null)
            {
                var binding = new GamepadInputBinding(waitKey.Value, e.Button);
                GameInput.AddBinding(binding);
                SetLabels(waitKey.Value);
                waitKey = null;
            }
        }

        private void btnSetUp_Click(object sender, EventArgs e)
        {
            if (waitKey != null)
            {
                SetLabels(waitKey.Value);
            }
            
            waitKey = GameInputs.Up;
            SetWaiting();
        }

        private void btnSetStart_Click(object sender, EventArgs e)
        {
            if (waitKey != null)
            {
                SetLabels(waitKey.Value);
            }
            
            waitKey = GameInputs.Start;
            SetWaiting();
        }

        private void btnSetSelect_Click(object sender, EventArgs e)
        {
            if (waitKey != null)
            {
                SetLabels(waitKey.Value);
            }
            
            waitKey = GameInputs.Select;
            SetWaiting();
        }

        private void btnSetShoot_Click(object sender, EventArgs e)
        {
            if (waitKey != null)
            {
                SetLabels(waitKey.Value);
            }
            
            waitKey = GameInputs.Shoot;
            SetWaiting();
        }

        private void btnSetRight_Click(object sender, EventArgs e)
        {
            if (waitKey != null)
            {
                SetLabels(waitKey.Value);
            }
            
            waitKey = GameInputs.Right;
            SetWaiting();
        }

        private void btnSetDown_Click(object sender, EventArgs e)
        {
            if (waitKey != null)
            {
                SetLabels(waitKey.Value);
            }
            
            waitKey = GameInputs.Down;
            SetWaiting();
        }

        private void btnSetJump_Click(object sender, EventArgs e)
        {
            if (waitKey != null)
            {
                SetLabels(waitKey.Value);
            }
            
            waitKey = GameInputs.Jump;
            SetWaiting();
        }

        private void btnSetLeft_Click(object sender, EventArgs e)
        {
            if (waitKey != null)
            {
                SetLabels(waitKey.Value);
            }
            
            waitKey = GameInputs.Left;
            SetWaiting();
        }

        private void SetWaiting()
        {
            if (waitKey != null)
            {
                var labels = Controls.OfType<Label>()
                    .Where(x => x.Name.ToUpper().Contains(waitKey.ToString().ToUpper()));

                foreach (var label in labels)
                {
                    label.Text = "Waiting...";
                }
            }
        }

        private void SetLabels(GameInputs input)
        {
            var types = new[] { "KEY", "PAD", "JOY" };
            var bindings = GameInput.GetBindings(input);
            var labels = Controls.OfType<Label>().Where(x => x.Name.ToUpper().Contains(input.ToString().ToUpper()));

            foreach (var t in types)
            {
                var typeLabel = labels.Single(x => x.Name.ToUpper().Contains(t));
                var typeBinding = bindings.FirstOrDefault(x => x.GetType().ToString().ToUpper().Contains(t));
                typeLabel.Text = typeBinding != null ? typeBinding.ToString() : "NONE";
            }
        }

        private void btnKeyboard_CheckedChanged(object sender, EventArgs e)
        {
            if (btnKeyboard.Checked)
                GameInput.ActiveType = InputTypes.Keyboard;
        }

        private void btnGamepad_CheckedChanged(object sender, EventArgs e)
        {
            if (btnGamepad.Checked)
                GameInput.ActiveType = InputTypes.Gamepad;
        }

        private void btnJoystick_CheckedChanged(object sender, EventArgs e)
        {
            if (btnJoystick.Checked)
                GameInput.ActiveType = InputTypes.Joystick;
        }
    }
}
