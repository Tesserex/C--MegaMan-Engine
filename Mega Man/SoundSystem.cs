using System;
using System.Collections.Generic;
using FMOD;
using System.Xml.Linq;
using MegaMan.Common;
using MegaManR.Audio;

namespace MegaMan.Engine
{
    public class SoundSystem : IDisposable
    {
        private readonly FMOD.System soundSystem;

        private readonly Dictionary<string, Music> loadedMusic = new Dictionary<string, Music>();
        private readonly Dictionary<string, ISoundEffect> loadedSounds = new Dictionary<string, ISoundEffect>();
        private readonly List<Channel> channels = new List<Channel>();
        private readonly System.Windows.Forms.Timer updateTimer;
        
        private BackgroundMusic bgm;
        private SoundEffect sfx;
        public static byte CurrentSfxPriority { get; set; }

        private bool musicEnabled = true;
        public bool MusicEnabled
        {
            get { return musicEnabled; }
            set 
            {
                musicEnabled = value;
                if (value) AudioManager.Instance.ResumeBGMPlayback();
                else AudioManager.Instance.PauseBGMPlayback();
            }
        }

        private bool sfxEnabled = true;
        public bool SfxEnabled
        {
            get { return sfxEnabled; }
            set
            {
                sfxEnabled = value;
                if (!value) AudioManager.Instance.StopSFXPlayback();
            }
        }

        public bool SquareOne
        {
            get { return AudioManager.Instance.Muted[0]; }
            set { AudioManager.Instance.MuteChannel(0, !value); }
        }

        public bool SquareTwo
        {
            get { return AudioManager.Instance.Muted[1]; }
            set { AudioManager.Instance.MuteChannel(1, !value); }
        }

        public bool Triangle
        {
            get { return AudioManager.Instance.Muted[2]; }
            set { AudioManager.Instance.MuteChannel(2, !value); }
        }

        public bool Noise
        {
            get { return AudioManager.Instance.Muted[3]; }
            set { AudioManager.Instance.MuteChannel(3, !value); }
        }

        public SoundSystem()
        {
            Factory.System_Create(ref soundSystem);
            uint version = 0;
            soundSystem.getVersion(ref version);
            soundSystem.init(32, INITFLAGS.NORMAL, (IntPtr)null);

            AudioManager.Instance.Initialize();
            AudioManager.Instance.Stereo = true;

            updateTimer = new System.Windows.Forms.Timer {Interval = 10};
            updateTimer.Tick += updateTimer_Tick;

            AudioManager.Instance.SFXPlaybackStopped += InstanceSfxPlaybackStopped;
            CurrentSfxPriority = 255;
        }

        static void InstanceSfxPlaybackStopped()
        {
            CurrentSfxPriority = 255;
        }

        public void Start()
        {
            updateTimer.Start();
            if (AudioManager.Instance.Paused) AudioManager.Instance.ResumeBGMPlayback();
        }

        public void Stop()
        {
            updateTimer.Stop();
            AudioManager.Instance.PauseBGMPlayback();
        }

        public void LoadEffectsFromXml(XElement node)
        {
            foreach (XElement soundNode in node.Elements("Sound"))
            {
                EffectFromXml(soundNode);
            }
        }

        public string EffectFromInfo(SoundInfo info)
        {
            if (loadedSounds.ContainsKey(info.Name)) return info.Name;

            ISoundEffect sound;
            if (info.Type == AudioType.Wav)
            {
                sound = new WavEffect(soundSystem, info.Path.Absolute, info.Loop, info.Volume);
            }
            else if (info.Type == AudioType.NSF)
            {
                sound = new NsfEffect(sfx, info.NsfTrack, info.Priority, info.Loop);
            }
            else return info.Name;

            loadedSounds[info.Name] = sound;
            return info.Name;
        }

        public string EffectFromXml(XElement soundNode)
        {
            string name = soundNode.RequireAttribute("name").Value;
            if (loadedSounds.ContainsKey(name)) return name;

            XAttribute pathattr = soundNode.Attribute("path");
            ISoundEffect sound;
            if (pathattr != null)
            {
                string path = System.IO.Path.Combine(Game.CurrentGame.BasePath, pathattr.Value);

                bool loop = soundNode.TryAttribute<bool>("loop");

                float vol = soundNode.TryAttribute<float>("volume", 1);

                sound = new WavEffect(soundSystem, path, loop, vol);
            }
            else
            {
                XAttribute trackAttr = soundNode.Attribute("track");
                if (trackAttr == null)
                {
                    // we trust that the sound they're talking about will be loaded eventually.
                    return name;
                }

                int track;
                if (!trackAttr.Value.TryParse(out track) || track <= 0) throw new GameXmlException(trackAttr, "Sound track attribute must be an integer greater than zero.");

                bool loop = soundNode.TryAttribute<bool>("loop");

                byte priority = soundNode.TryAttribute<byte>("priority", 100);

                sound = new NsfEffect(sfx, track, priority, loop);
            }
            loadedSounds[name] = sound;
            return name;
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
            AudioManager.Instance.StopBGMPlayback();
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
            sfx = new SoundEffect(AudioContainer.LoadContainer(path), 1);
            AudioManager.Instance.LoadSoundEffect(sfx);
        }

        public void PlayMusicNSF(uint track)
        {
            bgm.CurrentTrack = track-1;
            AudioManager.Instance.PlayBackgroundMusic(bgm);
            if (!MusicEnabled) AudioManager.Instance.PauseBGMPlayback();
        }

        public void PlaySfx(string name)
        {
            if (!SfxEnabled) return;

            if (loadedSounds.ContainsKey(name))
            {
                loadedSounds[name].Play();
            }
            else throw new GameRunException("Tried to play sound effect called " + name + ", but none was defined!");
        }

        public void StopMusicNsf()
        {
            AudioManager.Instance.StopBGMPlayback();
        }

        public void StopSfx(string name)
        {
            if (loadedSounds.ContainsKey(name))
            {
                loadedSounds[name].Stop();
                CurrentSfxPriority = 255;
            }
        }

        public void StopSfxIfLooping(string name)
        {
            if (loadedSounds.ContainsKey(name))
            {
                loadedSounds[name].StopIfLooping();
            }
        }
    }
}
