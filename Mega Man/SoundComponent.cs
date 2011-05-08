using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mega_Man
{
    public class SoundComponent : Component
    {
        private List<string> sounds = new List<string>();

        public override void LoadXml(XElement xml)
        {
            foreach (XElement soundNode in xml.Elements("Sound"))
            {
                string name = Engine.Instance.SoundSystem.EffectFromXml(soundNode);
                sounds.Add(name);
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
            foreach (string sound in sounds) Engine.Instance.SoundSystem.StopSfxNSF(sound);
        }

        public override void Message(IGameMessage msg)
        {
            SoundMessage sound = msg as SoundMessage;
            if (sound != null)
            {
                if (sound.Playing) Engine.Instance.SoundSystem.PlaySfx(sound.SoundName);
                else Engine.Instance.SoundSystem.StopSfxNSF(sound.SoundName);
            }
        }

        protected override void Update()
        {
            
        }

        public override void RegisterDependencies(Component component)
        {
            
        }

        public override Effect ParseEffect(XElement node)
        {
            string soundname = node.Attribute("name").Value;
            bool playing;
            if (!node.TryBool("playing", out playing)) playing = true;
            return (entity) =>
            {
                SoundMessage msg = new SoundMessage(entity, soundname, playing);
                entity.SendMessage(msg);
            };
        }
    }
}
