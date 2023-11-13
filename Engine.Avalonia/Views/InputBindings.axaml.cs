using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MegaMan.Engine.Avalonia.ViewModels;
using MegaMan.Engine.Input;

namespace MegaMan.Engine.Avalonia.Views
{
    public partial class InputBindings : Window
    {
        private InputBindingsViewModel vm;

        public InputBindings()
        {
            InitializeComponent();
            DataContext = vm = new InputBindingsViewModel();
            KeyDownEvent.AddClassHandler<TopLevel>(KeyPressed, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);
            Grid.AddHandler(KeyUpEvent, (s, e) => { e.Handled = true; }, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);

#if DEBUG
            this.AttachDevTools();
#endif
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            DeviceManager.Instance.JoystickButtonPressed += JoystickButtonPressed;
            DeviceManager.Instance.JoystickAxisPressed += JoystickAxisPressed;
            DeviceManager.Instance.GamepadButtonPressed += GamepadButtonPressed;
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void KeyPressed(TopLevel level, KeyEventArgs args)
        {
            vm.KeyPressed(args.Key);
            args.Handled = true;
#if DEBUG
            if (args.Key == Key.F12)
            {
                // allow dev tools to open
                args.Handled = false;
            }
#endif
        }

        private void JoystickButtonPressed(object? sender, JoystickButtonPressedEventArgs e)
        {
            vm.JoystickPressed(e.Button);
        }

        private void JoystickAxisPressed(object? sender, JoystickAxisPressedEventArgs e)
        {
            vm.JoystickPressed(e.Button);
        }

        private void GamepadButtonPressed(object? sender, GamepadButtonPressedEventArgs e)
        {
            vm.GamepadPressed(e.Button);
        }
    }
}
