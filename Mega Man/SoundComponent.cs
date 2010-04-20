using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mega_Man
{
    public class SoundComponent : Component
    {
        private Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();

        public void LoadXml(XElement xml)
        {
            foreach (XElement soundNode in xml.Elements("Sound"))
            {
                SoundEffect effect = Engine.Instance.SoundSystem.EffectFromXml(soundNode);
                sounds.Add(effect.Name, effect);
            }
        }

        public override Component Clone()
        {
            SoundComponent copy = new SoundComponent();
            copy.sounds = this.sounds;
            return copy;
        }

        public override void Start()
        {
            
        }

        public override void Stop()
        {
            foreach (SoundEffect sound in sounds.Values) sound.StopIfLooping();
        }

        public override void Message(IGameMessage msg)
        {
            SoundMessage sound = msg as SoundMessage;
            if (sound != null)
            {
                if (sounds.ContainsKey(sound.SoundName))
                {
                    if (sound.Playing) sounds[sound.SoundName].Play();
                    else sounds[sound.SoundName].Stop();
                }
                return;
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
