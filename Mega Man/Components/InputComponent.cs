using System.Collections.Generic;

namespace MegaMan.Engine
{
    public class InputComponent : Component
    {
        private readonly Dictionary<GameInput, bool> activeKeys = new Dictionary<GameInput, bool>();
        private readonly Dictionary<GameInput, bool> backupKeys = new Dictionary<GameInput, bool>();

        public bool Left { get { return KeyVal(GameInput.Left); } }
        public bool Right { get { return KeyVal(GameInput.Right); } }
        public bool Up { get { return KeyVal(GameInput.Up); } }
        public bool Down { get { return KeyVal(GameInput.Down); } }
        public bool Shoot { get; private set; }
        public bool ShootHeld { get { return KeyVal(GameInput.Shoot); } }
        public bool Jump { get; private set; }
        public bool JumpHeld { get { return KeyVal(GameInput.Jump); } }
        public bool StartKey { get; private set; }
        public bool Select { get; private set; }

        private bool paused;
        public bool Paused
        {
            get { return paused; }
            set
            {
                paused = value;
                if (value)
                {
                    backupKeys.Clear();
                    foreach (KeyValuePair<GameInput, bool> pair in activeKeys) backupKeys[pair.Key] = pair.Value;
                    activeKeys[GameInput.Left] = activeKeys[GameInput.Right] = activeKeys[GameInput.Up] = activeKeys[GameInput.Down] = false;
                }
                else // move backup to active
                {
                    activeKeys.Clear();
                    foreach (KeyValuePair<GameInput, bool> pair in backupKeys) activeKeys[pair.Key] = pair.Value;
                }
            }
        }

        private bool KeyVal(GameInput key)
        {
            return (!Paused && activeKeys.ContainsKey(key))? activeKeys[key] : false;
        }

        private void Reset()
        {
            Shoot = Jump = StartKey = Select = false;
            activeKeys.Clear();
            backupKeys.Clear();
        }

        public override Component Clone()
        {
            return new InputComponent();
        }

        public override void Start(IGameplayContainer container)
        {
            Reset();
            container.GameCleanup += Update;
            Engine.Instance.GameInputReceived += Instance_GameInputReceived;
        }

        public override void Stop(IGameplayContainer container)
        {
            Reset();
            container.GameCleanup -= Update;
            Engine.Instance.GameInputReceived -= Instance_GameInputReceived;
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
            var dict = Paused ? backupKeys : activeKeys;

            dict[e.Input] = e.Pressed;

            if (!Paused && !Parent.Paused)
            {
                switch (e.Input)
                {
                    case GameInput.Shoot:
                        Shoot = e.Pressed;
                        break;

                    case GameInput.Jump:
                        Jump = e.Pressed;
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
}
