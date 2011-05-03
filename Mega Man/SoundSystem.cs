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
        private Dictionary<string, ISoundEffect> loadedSounds = new Dictionary<string, ISoundEffect>();
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
            AudioManager.Instance.Stereo = true;

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

        public ISoundEffect EffectFromXml(XElement soundNode)
        {
            XAttribute pathattr = soundNode.Attribute("path");
            ISoundEffect sound;
            if (pathattr != null)
            {
                string path = System.IO.Path.Combine(Game.CurrentGame.BasePath, pathattr.Value);
                if (loadedSounds.ContainsKey(path)) return loadedSounds[path];

                bool loop = false;
                XAttribute loopAttr = soundNode.Attribute("loop");
                if (loopAttr != null)
                {
                    if (!bool.TryParse(loopAttr.Value, out loop)) throw new GameXmlException(loopAttr, "Sound loop attribute must be a boolean (true or false).");
                }

                float vol = 1;
                XAttribute volAttr = soundNode.Attribute("volume");
                if (volAttr != null)
                {
                    if (!volAttr.Value.TryParse(out vol)) throw new GameXmlException(volAttr, "Volume attribute must be a valid decimal.");
                }

                sound = new WavEffect(this.soundSystem, path, loop, vol);
                loadedSounds[path] = sound;
            }
            else
            {
                XAttribute trackAttr = soundNode.Attribute("track");
                if (trackAttr == null) throw new GameXmlException(soundNode, "Sound tag must include either a path attribute or a track attribute.");

                int track;
                if (!trackAttr.Value.TryParse(out track) || track <= 0) throw new GameXmlException(trackAttr, "Sound track attribute must be an integer greater than zero.");

                string key = "track" + track.ToString();
                if (loadedSounds.ContainsKey(key)) return loadedSounds[key];
                sound = new NsfEffect(this.sfx, track);
                loadedSounds[key] = sound;
            }
            return sound;
        }

        void updateTimer_Tick(object sender, EventArgs e)
        {
            if (soundSystem != null) soundSystem.update();
        }

        public void Unload()
        {
            foreach (Channel channel in channels) channel.stop();
            foreach (ISoundEffect sound in loadedSounds.Values) sound.Dispose();
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
            AudioManager.Instance.LoadBackgroundMusic(bgm);
        }

        public void LoadSfxNSF(string path)
        {
            sfx = new SoundEffect(AudioContainer.LoadContainer(path), SFXPlayType.Fired);
            AudioManager.Instance.LoadSoundEffect(sfx);
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
