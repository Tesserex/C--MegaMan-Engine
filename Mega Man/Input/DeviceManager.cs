﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;
using SharpDX.XInput;

namespace MegaMan.Engine.Input
{
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

        public event EventHandler<JoystickButtonPressedEventArgs> JoystickButtonPressed;
        public event EventHandler<JoystickAxisPressedEventArgs> JoystickAxisPressed;
        public event EventHandler<GamepadButtonPressedEventArgs> GamepadButtonPressed;

        private DeviceManager()
        {
            var directinput = new DirectInput();
            var joyInstances = directinput.GetDevices(SharpDX.DirectInput.DeviceType.Joystick, DeviceEnumerationFlags.AllDevices);

            this.joysticks = new List<Joystick>();
            foreach (var joy in joyInstances)
            {
                var stick = new Joystick(directinput, joy.InstanceGuid);
                stick.Properties.BufferSize = 128;
                stick.Acquire();
                this.joysticks.Add(stick);
            }

            this.controller = new Controller(UserIndex.One);

            Task.Factory.StartNew(PollJoysticks, TaskCreationOptions.LongRunning);
        }

        private void PollJoysticks()
        {
            while (true)
            {
                var tempButtons = new List<JoystickButton>();
                foreach (var stick in this.joysticks)
                {
                    stick.Poll();
                    var data = stick.GetBufferedData();
                    foreach (var update in data)
                    {
                        var button = new JoystickButton() { DeviceGuid = stick.Information.InstanceGuid, ButtonOffset = update.Offset };

                        if (update.Offset >= JoystickOffset.Buttons0 && update.Offset <= JoystickOffset.Buttons127)
                        {
                            if (update.Value == 128)
                                RaiseEventOnUIThread(JoystickButtonPressed, this, new JoystickButtonPressedEventArgs() { Button = button, Pressed = true });
                            else
                                RaiseEventOnUIThread(JoystickButtonPressed, this, new JoystickButtonPressedEventArgs() { Button = button, Pressed = false });
                        }
                        else if (update.Offset == JoystickOffset.X || update.Offset == JoystickOffset.Y)
                        {
                            if (update.Value == 0) // up, left
                            {   
                                RaiseEventOnUIThread(JoystickAxisPressed, this, new JoystickAxisPressedEventArgs() { Button = button, Value = -1 });
                            }
                            else if (update.Value == 65535) // down, right
                            {
                                RaiseEventOnUIThread(JoystickAxisPressed, this, new JoystickAxisPressedEventArgs() { Button = button, Value = 1 });
                            }
                            else
                            {
                                RaiseEventOnUIThread(JoystickAxisPressed, this, new JoystickAxisPressedEventArgs() { Button = button, Value = 0 });
                            }
                        }
                    }
                }

                if (this.controller.IsConnected)
                {
                    var padState = this.controller.GetState().Gamepad;
                    var buttonChanges = padState.Buttons ^ this.padButtonStates;

                    foreach (var button in Enum.GetValues(typeof(GamepadButtonFlags)).Cast<GamepadButtonFlags>())
                    {
                        if ((buttonChanges & button) > 0)
                        {
                            bool currentState = (padState.Buttons & button) > 0;
                            RaiseEventOnUIThread(GamepadButtonPressed, this, new GamepadButtonPressedEventArgs() { Button = button, Pressed = currentState });
                        }
                    }

                    this.padButtonStates = padState.Buttons;
                }
            }
        }

        private void RaiseEventOnUIThread(Delegate theEvent, params object[] args)
        {
            if (theEvent == null)
                return;

            foreach (Delegate d in theEvent.GetInvocationList())
            {
                ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                if (syncer == null)
                {
                    d.DynamicInvoke(args);
                }
                else
                {
                    if (!(d.Target is System.Windows.Forms.Control) || ((System.Windows.Forms.Control)d.Target).Created)
                        syncer.BeginInvoke(d, args);
                }
            }
        }
    }

    public class JoystickButtonPressedEventArgs
    {
        public JoystickButton Button { get; set; }
        public bool Pressed { get; set; }
    }

    public class JoystickAxisPressedEventArgs
    {
        public JoystickButton Button { get; set; }
        public sbyte Value { get; set; }
    }

    public class GamepadButtonPressedEventArgs
    {
        public GamepadButtonFlags Button { get; set; }
        public bool Pressed { get; set; }
    }

    public class JoystickButton
    {
        public Guid DeviceGuid { get; set; }
        public JoystickOffset ButtonOffset { get; set; }
    }
}