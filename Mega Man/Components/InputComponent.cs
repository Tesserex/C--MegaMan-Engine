using System.Collections.Generic;
using MegaMan.Engine.Input;

namespace MegaMan.Engine
{
    public class InputComponent : Component
    {
        private readonly Dictionary<GameInputs, bool> activeKeys = new Dictionary<GameInputs, bool>();
        private readonly Dictionary<GameInputs, bool> backupKeys = new Dictionary<GameInputs, bool>();

        public bool Left { get { return KeyVal(GameInputs.Left); } }
        public bool Right { get { return KeyVal(GameInputs.Right); } }
        public bool Up { get { return KeyVal(GameInputs.Up); } }
        public bool Down { get { return KeyVal(GameInputs.Down); } }
        public bool Shoot { get; private set; }
        public bool ShootHeld { get { return KeyVal(GameInputs.Shoot); } }
        public bool Jump { get; private set; }
        public bool JumpHeld { get { return KeyVal(GameInputs.Jump); } }
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
                    foreach (KeyValuePair<GameInputs, bool> pair in activeKeys) backupKeys[pair.Key] = pair.Value;
                    activeKeys[GameInputs.Left] = activeKeys[GameInputs.Right] = activeKeys[GameInputs.Up] = activeKeys[GameInputs.Down] = false;
                }
                else // move backup to active
                {
                    activeKeys.Clear();
                    foreach (KeyValuePair<GameInputs, bool> pair in backupKeys) activeKeys[pair.Key] = pair.Value;
                }
            }
        }

        private bool KeyVal(GameInputs key)
        {
            return (!Paused && activeKeys.ContainsKey(key))? activeKeys[key] : false;
        }

        private void Reset()
        {
            Shoot = Jump = StartKey = Select = false;
            activeKeys.Clear();
            backupKeys.Clear();

            // initialize
            var dict = Paused ? backupKeys : activeKeys;
            foreach (var k in globalKeys)
            {
                dict[k.Key] = k.Value;
            }
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
            globalKeys[e.Input] = e.Pressed;

            if (!Paused && !Parent.Paused)
            {
                switch (e.Input)
                {
                    case GameInputs.Shoot:
                        Shoot = e.Pressed;
                        break;

                    case GameInputs.Jump:
                        Jump = e.Pressed;
                        break;

                    case GameInputs.Start:
                        StartKey = e.Pressed;
                        break;

                    case GameInputs.Select:
                        Select = e.Pressed;
                        break;
                }
            }
        }

        private static readonly Dictionary<GameInputs, bool> globalKeys = new Dictionary<GameInputs, bool>();
    }
}
