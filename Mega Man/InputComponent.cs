using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mega_Man
{
    public class InputComponent : Component
    {
        public bool Left { get; private set; }
        public bool Right { get; private set; }
        public bool Up { get; private set; }
        public bool Down { get; private set; }
        public bool Shoot { get; private set; }
        public bool ShootHeld { get; private set; }
        public bool Jump { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool StartKey { get; private set; }
        public bool Select { get; private set; }

        private static InputComponent instance;
        public static InputComponent Get()
        {
            if (instance == null) instance = new InputComponent();
            return instance;
        }

        public InputComponent()
        {
            Engine.Instance.GameInputReceived += new GameInputEventHandler(Instance_GameInputReceived);
        }

        public override Component Clone()
        {
            return this;
        }

        public override void Start()
        {
            Engine.Instance.GameThink += Update;
        }

        public override void Stop()
        {
            Engine.Instance.GameThink -= Update;
        }

        public override void Message(IGameMessage msg) { }

        protected override void Update()
        {
            // these are the things that are only true on the frame in which they are pressed
            // so, we reset them here to prevent multiple firings
            Jump = Shoot = StartKey = Select = false;
        }

        public override void RegisterDependencies(Component component)
        {

        }

        private void Instance_GameInputReceived(GameInputEventArgs e)
        {
            switch (e.Input)
            {
                case GameInput.Up:
                    Up = e.Pressed;
                    break;

                case GameInput.Down:
                    Down = e.Pressed;
                    break;

                case GameInput.Left:
                    Left = e.Pressed;
                    break;

                case GameInput.Right:
                    Right = e.Pressed;
                    break;

                case GameInput.Shoot:
                    Shoot = ShootHeld = e.Pressed;
                    break;

                case GameInput.Jump:
                    Jump = JumpHeld = e.Pressed;
                    break;

                case GameInput.Start:
                    StartKey = e.Pressed;
                    break;

                case GameInput.Select:
                    Select = e.Pressed;
                    break;
            }
        }
    }
}
