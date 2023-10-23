using SharpDX.DirectInput;
using SharpDX.XInput;
using DeviceType = SharpDX.DirectInput.DeviceType;

namespace MegaMan.Engine.Input
{
    public delegate void RaiseEventOnUIThreadCallback(Delegate theEvent, params object[] args);

    public class DeviceManager
    {
        private static DeviceManager instance;

        public static DeviceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DeviceManager();
                }
                return instance;
            }
        }

        private Controller controller;
        private GamepadButtonFlags padButtonStates;
        private List<Joystick> joysticks;

        private RaiseEventOnUIThreadCallback? RaiseEventOnUIThread;

        public event EventHandler<JoystickButtonPressedEventArgs>? JoystickButtonPressed;
        public event EventHandler<JoystickAxisPressedEventArgs>? JoystickAxisPressed;
        public event EventHandler<GamepadButtonPressedEventArgs>? GamepadButtonPressed;

        private DeviceManager()
        {
            var directinput = new DirectInput();
            var joyInstances = directinput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices);

            joysticks = new List<Joystick>();
            foreach (var joy in joyInstances)
            {
                var stick = new Joystick(directinput, joy.InstanceGuid);
                stick.Properties.BufferSize = 128;
                stick.Acquire();
                joysticks.Add(stick);
            }

            controller = new Controller(UserIndex.One);
        }

        public void Start(RaiseEventOnUIThreadCallback raiseEventOnUIThreadCallback)
        {
            RaiseEventOnUIThread = raiseEventOnUIThreadCallback;
            Task.Factory.StartNew(PollJoysticks, TaskCreationOptions.LongRunning);
        }

        private void PollJoysticks()
        {
            while (true)
            {
                var tempButtons = new List<JoystickButton>();
                foreach (var stick in joysticks)
                {
                    stick.Poll();
                    var data = stick.GetBufferedData();
                    foreach (var update in data)
                    {
                        var button = new JoystickButton { DeviceGuid = stick.Information.InstanceGuid, ButtonOffset = update.Offset };

                        if (update.Offset >= JoystickOffset.Buttons0 && update.Offset <= JoystickOffset.Buttons127)
                        {
                            if (update.Value == 128)
                                RaiseEventOnUIThread(JoystickButtonPressed, this, new JoystickButtonPressedEventArgs { Button = button, Pressed = true });
                            else
                                RaiseEventOnUIThread(JoystickButtonPressed, this, new JoystickButtonPressedEventArgs { Button = button, Pressed = false });
                        }
                        else if (update.Offset == JoystickOffset.X || update.Offset == JoystickOffset.Y)
                        {
                            if (update.Value == 0) // up, left
                            {   
                                RaiseEventOnUIThread(JoystickAxisPressed, this, new JoystickAxisPressedEventArgs { Button = button, Value = -1 });
                            }
                            else if (update.Value == 65535) // down, right
                            {
                                RaiseEventOnUIThread(JoystickAxisPressed, this, new JoystickAxisPressedEventArgs { Button = button, Value = 1 });
                            }
                            else
                            {
                                RaiseEventOnUIThread(JoystickAxisPressed, this, new JoystickAxisPressedEventArgs { Button = button, Value = 0 });
                            }
                        }
                    }
                }

                if (controller.IsConnected)
                {
                    var padState = controller.GetState().Gamepad;
                    var buttonChanges = padState.Buttons ^ padButtonStates;

                    foreach (var button in Enum.GetValues(typeof(GamepadButtonFlags)).Cast<GamepadButtonFlags>())
                    {
                        if ((buttonChanges & button) > 0)
                        {
                            var currentState = (padState.Buttons & button) > 0;
                            RaiseEventOnUIThread(GamepadButtonPressed, this, new GamepadButtonPressedEventArgs { Button = button, Pressed = currentState });
                        }
                    }

                    padButtonStates = padState.Buttons;
                }
            }
        }
    }
}
