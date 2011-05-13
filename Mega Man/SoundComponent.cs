using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan;

namespace Mega_Man
{
    public class SoundComponent : Component
    {
        private HashSet<string> sounds = new HashSet<string>();

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
            foreach (string sound in sounds) Engine.Instance.SoundSystem.StopSfxIfLooping(sound);
        }

        public override void Message(IGameMessage msg)
        {
            SoundMessage sound = msg as SoundMessage;
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
