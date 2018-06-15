using System.Collections.Generic;

namespace MegaMan.Engine
{
    public class SoundComponent : Component
    {
        private HashSet<string> sounds = new HashSet<string>();
        
        public override Component Clone()
        {
            return this;
        }

        public override void Start(IGameplayContainer container)
        {
            
        }

        public override void Stop(IGameplayContainer container)
        {
            foreach (var sound in sounds) Engine.Instance.SoundSystem.StopSfxIfLooping(sound);
        }

        public override void Message(IGameMessage msg)
        {
            var sound = msg as SoundMessage;
            if (sound != null)
            {
                sounds.Add(sound.SoundName);
                if (sound.Playing) Engine.Instance.SoundSystem.PlaySfx(sound.SoundName);
                else Engine.Instance.SoundSystem.StopSfx(sound.SoundName);
            }
        }

        protected override void Update()
        {
            
        }

        public override void RegisterDependencies(Component component)
        {
            
        }
    }
}
