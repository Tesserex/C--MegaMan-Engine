using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.DirectInput;
using SharpDX.XInput;

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

    public enum InputTypes
    {
        Keyboard,
        Gamepad,
        Joystick
    }

    public static class GameInput
    {
        // this holds the key pressed state of all input keys, so that when they change,
        // they can be translated into a GameInput event.
        private static readonly Dictionary<GameInputs, bool> inputFlags = new Dictionary<GameInputs, bool>();

        private static List<IGameInputBinding> bindings = new List<IGameInputBinding>();

        public static InputTypes ActiveType { get; set; }

        public static void AddBinding(IGameInputBinding binding)
        {
            bindings.RemoveAll(x => x.Input == binding.Input && x.GetType() == binding.GetType());
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

        public static IEnumerable<GamepadInputBinding> GetGamepadBindings()
        {
            return bindings.OfType<GamepadInputBinding>();
        }

        public static Dictionary<GameInputs, bool> GetChangedInputs()
        {
            var result = new Dictionary<GameInputs, bool>();

            var active = bindings.Where(x => x.InputType == ActiveType);
            foreach (var binding in active)
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
            DeviceManager.Instance.GamepadButtonPressed += GamepadButtonPressed;
        }

        private static void GamepadButtonPressed(object sender, GamepadButtonPressedEventArgs e)
        {
            var matches = bindings.OfType<GamepadInputBinding>()
                .Where(b => b.Button == e.Button);

            foreach (var binding in matches)
            {
                binding.IsPressed = e.Pressed;
            }
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
        InputTypes InputType { get; }
        bool IsPressed { get; }
    }

    public class KeyboardInputBinding : IGameInputBinding
    {
        public GameInputs Input { get; private set; }
        public InputTypes InputType { get { return InputTypes.Keyboard; } }
        public System.Windows.Forms.Keys Key { get; private set; }

        public KeyboardInputBinding(GameInputs input, System.Windows.Forms.Keys key)
        {
            this.Input = input;
            this.Key = key;
        }

        public bool IsPressed { get { return Program.KeyDown(this.Key); } }

        public override string ToString()
        {
            return this.Key.ToString();
        }
    }

    public class JoystickInputBinding : IGameInputBinding
    {
        public GameInputs Input { get; private set; }
        public InputTypes InputType { get { return InputTypes.Joystick; } }
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
            return string.Format("{0} {1}", this.Button.ToString(), this.Value != 0 ? this.Value.ToString() : "");
        }
    }

    public class GamepadInputBinding : IGameInputBinding
    {
        public GameInputs Input { get; private set; }
        public InputTypes InputType { get { return InputTypes.Gamepad; } }
        public GamepadButtonFlags Button { get; private set; }

        public GamepadInputBinding(GameInputs input, GamepadButtonFlags button)
        {
            this.Input = input;
            this.Button = button;
        }

        public bool IsPressed { get; set; }

        public override string ToString()
        {
            return this.Button.ToString();
        }
    }
}
