using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD;
using System.Xml.Linq;
using MegaMan;
using MegaManR.Audio;
using MegaManR.Extensions;

namespace Mega_Man
{
    public class SoundSystem : IDisposable
    {
        private FMOD.System soundSystem;

        private Dictionary<string, Music> loadedMusic = new Dictionary<string, Music>();
        private Dictionary<string, WavEffect> loadedSounds = new Dictionary<string, WavEffect>();
        private List<int> playCount = new List<int>();
        private List<Channel> channels = new List<Channel>();
        private System.Windows.Forms.Timer updateTimer;

        private BackgroundMusic bgm;
        private SoundEffect sfx;

        public SoundSystem()
        {
            FMOD.Factory.System_Create(ref soundSystem);
            uint version = 0;
            soundSystem.getVersion(ref version);
            soundSystem.init(32, FMOD.INITFLAGS.NORMAL, (IntPtr)null);

            AudioManager.Instance.Initialize();

            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 10;
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
        }

        public void Start()
        {
            updateTimer.Start();
            if (AudioManager.Instance.Paused) AudioManager.Instance.ResumePlayback();
        }

        public void Stop()
        {
            updateTimer.Stop();
            AudioManager.Instance.PausePlayback();
        }

        public WavEffect EffectFromXml(XElement soundNode)
        {
            XAttribute pathattr = soundNode.Attribute("path");
            if (pathattr == null) throw new EntityXmlException(soundNode, "Sounds must give a path to the file!");

            string path = System.IO.Path.Combine(Game.CurrentGame.BasePath, pathattr.Value);
            if (loadedSounds.ContainsKey(path)) return loadedSounds[path];

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
                if (!volAttr.Value.TryParse(out vol)) throw new EntityXmlException(volAttr, "Volume attribute must be a valid decimal.");
            }

            WavEffect sound = new WavEffect(this.soundSystem, path, loop, vol);
            XAttribute nameattr = soundNode.Attribute("name");
            if (nameattr != null) sound.Name = nameattr.Value;

            loadedSounds[path] = sound;
            return sound;
        }

        void updateTimer_Tick(object sender, EventArgs e)
        {
            if (soundSystem != null) soundSystem.update();
        }

        public void Unload()
        {
            foreach (Channel channel in channels) channel.stop();
            foreach (WavEffect sound in loadedSounds.Values) sound.Dispose();
            foreach (Music music in loadedMusic.Values) music.Dispose();
            loadedSounds.Clear();
            channels.Clear();
            loadedMusic.Clear();
            AudioManager.Instance.StopPlayback();
            if (bgm != null) bgm.Release();
            if (sfx != null) sfx.Release();
        }

        public void Dispose()
        {
            Unload();
            soundSystem.release();
        }

        public Music LoadMusic(string intro, string loop, float volume)
        {
            string key = intro + loop;

            if (!string.IsNullOrEmpty(key) && loadedMusic.ContainsKey(intro + loop)) return loadedMusic[intro + loop];

            Music music = new Music(soundSystem, intro, loop, volume);
            loadedMusic[intro + loop] = music;
            return music;
        }

        public void LoadMusicNSF(string path)
        {
            bgm = new BackgroundMusic(AudioContainer.LoadContainer(path));
        }

        public void LoadSfxNSF(string path)
        {
            sfx = new SoundEffect(AudioContainer.LoadContainer(path), SFXPlayType.Fired);
        }

        public void PlayNSF(uint track)
        {
            bgm.CurrentTrack = track-1;
            AudioManager.Instance.PlayBackgroundMusic(bgm);
        }

        public void StopNSF()
        {
            AudioManager.Instance.StopPlayback();
        }
    }
}
