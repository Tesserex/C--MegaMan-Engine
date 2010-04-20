using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mega_Man
{
    public class SoundComponent : Component
    {
        private Dictionary<string, int> sounds = new Dictionary<string,int>();

        public void LoadXml(XElement xml)
        {
            foreach (XElement soundNode in xml.Elements("Sound"))
            {
                XAttribute nameattr = soundNode.Attribute("name");
                if (nameattr == null) continue;
                XAttribute pathattr = soundNode.Attribute("path");
                if (pathattr == null) continue;
                bool loop = false;
                XAttribute loopAttr = soundNode.Attribute("loop");
                if (loopAttr != null)
                {
                    if (!bool.TryParse(loopAttr.Value, out loop)) throw new EntityXmlException(loopAttr, "Sound loop attribute must be a boolean (true or false).");
                }
                float vol = 1;
                XAttribute volAttr = soundNode.Attribute("volume");
                if (volAttr != null)
                {
                    if (!float.TryParse(volAttr.Value, out vol)) throw new EntityXmlException(volAttr, "Volume attribute must be a valid decimal.");
                }
                AddSound(nameattr.Value, System.IO.Path.Combine(Game.CurrentGame.BasePath, pathattr.Value), loop, vol);
            }
        }

        public void AddSound(string name, string path, bool loop, float volume)
        {
            int handle = Engine.Instance.LoadSoundEffect(path, loop, volume);
            sounds.Add(name, handle);
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
            foreach (int sound in sounds.Values) Engine.Instance.StopSoundIfLoop(sound);
        }

        public override void Message(IGameMessage msg)
        {
            SoundMessage sound = msg as SoundMessage;
            if (sound != null)
            {
                if (sounds.ContainsKey(sound.SoundName))
                {
                    if (sound.Playing) Engine.Instance.PlaySound(sounds[sound.SoundName]);
                    else Engine.Instance.StopSound(sounds[sound.SoundName]);
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
