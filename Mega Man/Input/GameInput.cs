using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Input;
using SharpDX.DirectInput;

namespace MegaMan.Engine.Input
{
    public enum GameInputs
    {
        Up,
        Down,
        Left,
        Right,
        Jump,
        Shoot,
        Start,
        Select,
        None
    }

    public static class GameInput
    {
        // this holds the key pressed state of all input keys, so that when they change,
        // they can be translated into a GameInput event.
        private static readonly Dictionary<GameInputs, bool> inputFlags = new Dictionary<GameInputs, bool>();

        private static List<IGameInputBinding> bindings = new List<IGameInputBinding>();

        public static void AddBinding(IGameInputBinding binding)
        {
            bindings.Add(binding);
        }

        public static void ClearBinding(GameInputs input)
        {
            bindings.RemoveAll(b => b.Input == input);
        }

        public static IEnumerable<IGameInputBinding> GetBindings(GameInputs input)
        {
            return bindings.Where(b => b.Input == input);
        }

        public static IEnumerable<KeyboardInputBinding> GetKeyBindings()
        {
            return bindings.OfType<KeyboardInputBinding>();
        }

        public static IEnumerable<JoystickInputBinding> GetJoystickBindings()
        {
            return bindings.OfType<JoystickInputBinding>();
        }

        public static Dictionary<GameInputs, bool> GetChangedInputs()
        {
            var result = new Dictionary<GameInputs, bool>();

            foreach (var binding in bindings)
            {
                if (binding.IsPressed)
                {
                    if (!inputFlags.ContainsKey(binding.Input) || inputFlags[binding.Input] == false)
                    {
                        inputFlags[binding.Input] = true;
                        result[binding.Input] = true;
                    }
                }
                else if (inputFlags.ContainsKey(binding.Input) && inputFlags[binding.Input])
                {
                    inputFlags[binding.Input] = false;
                    result[binding.Input] = false;
                }
            }

            return result;
        }

        static GameInput()
        {
            DeviceManager.Instance.JoystickButtonPressed += JoystickButtonPressed;
            DeviceManager.Instance.JoystickAxisPressed += JoystickAxisPressed;
        }

        private static void JoystickAxisPressed(object sender, JoystickAxisPressedEventArgs e)
        {
            var matches = bindings.OfType<JoystickInputBinding>()
                .Where(b => b.DeviceGuid == e.Button.DeviceGuid && b.Button == e.Button.ButtonOffset);

            foreach (var binding in matches)
            {
                binding.IsPressed = (binding.Value == e.Value);
            }
        }

        private static void JoystickButtonPressed(object sender, JoystickButtonPressedEventArgs e)
        {
            var matches = bindings.OfType<JoystickInputBinding>()
                .Where(b => b.DeviceGuid == e.Button.DeviceGuid && b.Button == e.Button.ButtonOffset);

            foreach (var binding in matches)
            {
                binding.IsPressed = e.Pressed;
            }
        }
    }

    public interface IGameInputBinding
    {
        GameInputs Input { get; }
        bool IsPressed { get; }
    }

    public class KeyboardInputBinding : IGameInputBinding
    {
        public GameInputs Input { get; private set; }
        public System.Windows.Forms.Keys Key { get; private set; }

        public KeyboardInputBinding(GameInputs input, System.Windows.Forms.Keys key)
        {
            this.Input = input;
            this.Key = key;
        }

        public bool IsPressed { get { return Program.KeyDown(this.Key); } }

        public override string ToString()
        {
            return "Key " + this.Key.ToString();
        }
    }

    public class JoystickInputBinding : IGameInputBinding
    {
        public GameInputs Input { get; private set; }
        public JoystickOffset Button { get; private set; }
        public Guid DeviceGuid { get; private set; }
        public int Value { get; private set; }

        public JoystickInputBinding(GameInputs input, Guid guid, JoystickOffset button, int value = 0)
        {
            this.Input = input;
            this.DeviceGuid = guid;
            this.Button = button;
            this.Value = value;
        }

        public bool IsPressed { get; set; }

        public override string ToString()
        {
            return string.Format("Joystick {0} {1}", this.Button.ToString(), this.Value != 0 ? this.Value.ToString() : "");
        }
    }

    public class GamepadInputBinding : IGameInputBinding
    {
        public GameInputs Input { get; private set; }
        public Buttons Button { get; private set; }

        public GamepadInputBinding(GameInputs input, Buttons button)
        {
            this.Input = input;
            this.Button = button;
        }

        public bool IsPressed
        {
            get
            {
                var capabilities = GamePad.GetCapabilities(Microsoft.Xna.Framework.PlayerIndex.One);
                if (capabilities.IsConnected)
                {
                    return GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One).IsButtonDown(this.Button);
                }
                return false;
            }
        }

        public override string ToString()
        {
            return "Gamepad " + this.Button.ToString();
        }
    }
}
