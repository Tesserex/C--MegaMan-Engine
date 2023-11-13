using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MegaMan.Engine.Avalonia.Settings;
using MegaMan.Engine.Input;
using SharpDX.XInput;

namespace MegaMan.Engine.Avalonia.ViewModels
{
    internal class InputBindingsViewModel : ViewModelBase
    {
        private GameInputs? waitKey;

        public ObservableCollection<BindingViewModel> Input { get; private set; }

        public ICommand ChangeCommand { get; }

        public InputBindingsViewModel()
        {
            Input = new ObservableCollection<BindingViewModel>(Enum.GetValues<GameInputs>().Select(input => {
                var binds = GameInput.GetBindings(input);
                var vm = new BindingViewModel() {
                    Input = input,
                    Key = binds.OfType<AvaloniaKeyboardInputBinding>().FirstOrDefault()?.Key,
                    XBox = binds.OfType<GamepadInputBinding>().FirstOrDefault()?.Button,
                    Joystick = binds.OfType<JoystickInputBinding>().FirstOrDefault()?.Button
                };
                return vm;
            }));

            OnPropertyChanged(nameof(Input));

            ChangeCommand = new RelayCommand<GameInputs>(Change, (i) => waitKey is null);
        }

        private void Change(GameInputs input)
        {
            waitKey = input;
            var existing = Input.FirstOrDefault(vm => vm.Input == waitKey.Value);
            if (existing != null)
            {
                existing.Waiting = true;
            }
            else
            {
                Input.Add(new BindingViewModel() {
                    Input = waitKey.Value,
                    Waiting = true
                });
            }
        }

        public void KeyPressed(Key key)
        {
            if (waitKey != null)
            {
                var binding = new AvaloniaKeyboardInputBinding(waitKey.Value, key);
                GameInput.AddBinding(binding);
                var existing = Input.FirstOrDefault(vm => vm.Input == waitKey.Value);
                if (existing != null)
                {
                    existing.Key = key;
                    existing.Waiting = false;
                }
                else
                {
                    Input.Add(new BindingViewModel() {
                        Input = waitKey.Value,
                        Key = key
                    });
                }
                waitKey = null;
            }
        }

        public void JoystickPressed(JoystickButton button)
        {
            if (waitKey != null)
            {
                var binding = new JoystickInputBinding(waitKey.Value, button.DeviceGuid, button.ButtonOffset);
                GameInput.AddBinding(binding);
                var existing = Input.FirstOrDefault(vm => vm.Input == waitKey.Value);
                if (existing != null)
                {
                    existing.Joystick = button.ButtonOffset;
                    existing.Waiting = false;
                }
                else
                {
                    Input.Add(new BindingViewModel() {
                        Input = waitKey.Value,
                        Joystick = button.ButtonOffset
                    });
                }
                waitKey = null;
            }
        }

        public void GamepadPressed(GamepadButtonFlags button)
        {
            if (waitKey != null)
            {
                var binding = new GamepadInputBinding(waitKey.Value, button);
                GameInput.AddBinding(binding);
                var existing = Input.FirstOrDefault(vm => vm.Input == waitKey.Value);
                if (existing != null)
                {
                    existing.XBox = button;
                    existing.Waiting = false;
                }
                else
                {
                    Input.Add(new BindingViewModel() {
                        Input = waitKey.Value,
                        XBox = button
                    });
                }
                waitKey = null;
            }
        }
    }

    internal class BindingViewModel : ObservableObject
    {
        private GameInputs input;
        public GameInputs Input { get => input; set { SetProperty(ref input, value); } }

        private Key? key;
        public Key? Key { get => key; set { SetProperty(ref key, value); } }
        public string KeyString { get => waiting ? "Waiting..." : key?.ToString() ?? "NONE"; }

        private GamepadButtonFlags? xbox;
        public GamepadButtonFlags? XBox { get => xbox; set { SetProperty(ref xbox, value); } }
        public string XBoxString { get => waiting ? "Waiting..." : xbox?.ToString() ?? "NONE"; }

        private SharpDX.DirectInput.JoystickOffset? joystick;
        public SharpDX.DirectInput.JoystickOffset? Joystick { get => joystick; set { SetProperty(ref joystick, value); } }
        public string JoystickString { get => waiting ? "Waiting..." : joystick?.ToString() ?? "NONE"; }

        private bool waiting;
        public bool Waiting {
            get => waiting;
            set {
                waiting = value;
                OnPropertyChanged(nameof(KeyString));
                OnPropertyChanged(nameof(XBoxString));
                OnPropertyChanged(nameof(JoystickString));
            }
        }
    }
}
