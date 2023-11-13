using System;
using Avalonia.Controls;
using Avalonia.Input;
using MegaMan.Engine.Input;
using Microsoft.Xna.Framework.Input;

namespace MegaMan.Engine.Avalonia.Settings
{
    internal class AvaloniaKeyboardInputBinding : IGameInputBinding
    {
        public GameInputs Input { get; private set; }
        public InputTypes InputType { get { return InputTypes.Keyboard; } }
        public Key Key { get; private set; }

        public AvaloniaKeyboardInputBinding(GameInputs input, Key key)
        {
            Input = input;
            Key = key;

            InputElement.KeyDownEvent.AddClassHandler<TopLevel>(OnKeyDown, handledEventsToo: true);
            InputElement.KeyUpEvent.AddClassHandler<TopLevel>(OnKeyUp, handledEventsToo: true);
        }

        public bool IsPressed { get; private set; }

        public override string ToString()
        {
            return Key.ToString();
        }

        private void OnKeyDown(object s, KeyEventArgs e)
        {
            if (e.Key == Key) IsPressed = true;
        }

        private void OnKeyUp(object s, KeyEventArgs e)
        {
            if (e.Key == Key) IsPressed = false;
        }
    }
}
