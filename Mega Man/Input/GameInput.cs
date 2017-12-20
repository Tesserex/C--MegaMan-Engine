using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private static Dictionary<GameInputs, IGameInputBinding> bindings = new Dictionary<GameInputs, IGameInputBinding>();

        public static void SetBinding(GameInputs input, IGameInputBinding binding)
        {
            bindings[input] = binding;
        }

        public static IGameInputBinding GetBinding(GameInputs input)
        {
            return bindings.ContainsKey(input) ? bindings[input] : null;
        }

        public static Dictionary<GameInputs, bool> GetChangedInputs()
        {
            var result = new Dictionary<GameInputs, bool>();

            foreach (var input in bindings.Keys)
            {
                var binding = bindings[input];
                if (binding.IsPressed)
                {
                    if (!inputFlags.ContainsKey(input) || inputFlags[input] == false)
                    {
                        inputFlags[input] = true;
                        result[input] = true;
                    }
                }
                else if (inputFlags.ContainsKey(input) && inputFlags[input])
                {
                    inputFlags[input] = false;
                    result[input] = false;
                }
            }

            return result;
        }
    }

    public interface IGameInputBinding
    {
        bool IsPressed { get; }
    }

    public class KeyboardInputBinding : IGameInputBinding
    {
        public Keys Key { get; private set; }

        public KeyboardInputBinding(Keys key)
        {
            this.Key = key;
        }

        public bool IsPressed { get { return Program.KeyDown(this.Key); } }

        public override string ToString()
        {
            return this.Key.ToString();
        }
    }
}
