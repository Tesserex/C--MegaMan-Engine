using FMOD;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaManR.Audio;

namespace MegaMan.Engine
{
    public interface ISoundSystem
    {
        bool MusicEnabled { get; set; }
        bool Noise { get; set; }
        bool SfxEnabled { get; set; }
        bool SquareOne { get; set; }
        bool SquareTwo { get; set; }
        bool Triangle { get; set; }
        int Volume { get; set; }

        void ApplyMusicSetting();
        void Dispose();
        string EffectFromInfo(SoundInfo info);
        void LoadEffectsFromInfo(IEnumerable<SoundInfo> sounds);
        Music LoadMusic(string intro, string loop, float volume);
        void LoadMusicNSF(byte[] nsfData);
        void LoadSfxNSF(byte[] nsfData);
        void PlayMusicNSF(uint track);
        void PlaySfx(string name);
        void Start();
        void Stop();
        void StopMusicNsf();
        void StopSfx(string name);
        void StopSfxIfLooping(string name);
        void Tick();
        void Unload();
    }

    public class FakeSoundSystem : ISoundSystem, IDisposable
    {
        public bool MusicEnabled { get => false; set { } }
        public bool Noise { get => false; set { } }
        public bool SfxEnabled { get => false; set { } }
        public bool SquareOne { get => false; set { } }
        public bool SquareTwo { get => false; set { } }
        public bool Triangle { get => false; set { } }
        public int Volume { get => 0; set { } }

        public void ApplyMusicSetting() { }

        public void Dispose() { }

        public string EffectFromInfo(SoundInfo info) => "";

        public void LoadEffectsFromInfo(IEnumerable<SoundInfo> sounds) { }

        public Music LoadMusic(string intro, string loop, float volume) => null;

        public void LoadMusicNSF(byte[] nsfData) { }

        public void LoadSfxNSF(byte[] nsfData) { }

        public void PlayMusicNSF(uint track) { }

        public void PlaySfx(string name) { }

        public void Start() { }

        public void Stop() { }

        public void StopMusicNsf() { }

        public void StopSfx(string name) { }

        public void StopSfxIfLooping(string name) { }

        public void Tick() { }

        public void Unload() { }
    }

    public class SoundSystem : IDisposable, ISoundSystem
    {
        private bool initialized;
        private readonly FMOD.System soundSystem;

        private readonly Dictionary<string, Music> loadedMusic = new Dictionary<string, Music>();
        private readonly Dictionary<string, ISoundEffect> loadedSounds = new Dictionary<string, ISoundEffect>();
        private readonly List<Channel> channels = new List<Channel>();

        private BackgroundMusic? bgm;
        private SoundEffect? sfx;
        public static byte CurrentSfxPriority { get; set; }

        private bool musicEnabled = true;
        public bool MusicEnabled
        {
            get { return musicEnabled; }
            set
            {
                if (musicEnabled != value)
                {
                    musicEnabled = value;
                    if (Engine.Instance.IsRunning)
                        ApplyMusicSetting();
                }
            }
        }

        private int volume;
        public int Volume
        {
            get { return volume; }
            set
            {
                volume = Math.Max(0, Math.Min(100, value));
                if (initialized) AudioManager.Instance.ChangeVolume(volume / 100f);
            }
        }

        private bool sfxEnabled = true;
        public bool SfxEnabled
        {
            get { return sfxEnabled; }
            set
            {
                sfxEnabled = value;
                if (!value && initialized) AudioManager.Instance.StopSFXPlayback();
            }
        }

        public bool SquareOne
        {
            get { return !AudioManager.Instance.Muted[0]; }
            set { if (bgm != null && initialized) AudioManager.Instance.MuteChannel(0, !value); }
        }

        public bool SquareTwo
        {
            get { return !AudioManager.Instance.Muted[1]; }
            set { if (bgm != null && initialized) AudioManager.Instance.MuteChannel(1, !value); }
        }

        public bool Triangle
        {
            get { return !AudioManager.Instance.Muted[2]; }
            set { if (bgm != null && initialized) AudioManager.Instance.MuteChannel(2, !value); }
        }

        public bool Noise
        {
            get { return !AudioManager.Instance.Muted[3]; }
            set { if (bgm != null && initialized) AudioManager.Instance.MuteChannel(3, !value); }
        }

        public SoundSystem()
        {
            Factory.System_Create(ref soundSystem);
            uint version = 0;
            soundSystem.getVersion(ref version);
            soundSystem.init(32, INITFLAGS.NORMAL, (IntPtr)null);

            AudioManager.Instance.SFXPlaybackStopped += InstanceSfxPlaybackStopped;
            CurrentSfxPriority = 255;
        }

        static void InstanceSfxPlaybackStopped()
        {
            CurrentSfxPriority = 255;
        }

        public void Start()
        {
            // the AudioManager has a bug where it never sets Initialized to true, so use our own
            if (!initialized)
            {
                AudioManager.Instance.Initialize();
                AudioManager.Instance.Stereo = true;
                initialized = true;

                // this looks really dumb but we want the side effects that actually initialize values in the audio manager
                Volume = Volume;
                SfxEnabled = sfxEnabled;
                SquareOne = SquareOne;
                SquareTwo = SquareTwo;
                Triangle = Triangle;
                Noise = Noise;
            }

            if (AudioManager.Instance.Paused)
                ApplyMusicSetting();
        }

        public void Stop()
        {
            AudioManager.Instance.PauseBGMPlayback();
        }

        public void ApplyMusicSetting()
        {
            if (MusicEnabled)
                AudioManager.Instance.ResumeBGMPlayback();
            else
                AudioManager.Instance.PauseBGMPlayback();
        }

        public void LoadEffectsFromInfo(IEnumerable<SoundInfo> sounds)
        {
            foreach (var sound in sounds)
            {
                EffectFromInfo(sound);
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
            else if (info.Type == AudioType.Nsf)
            {
                sound = new NsfEffect(sfx, info.NsfTrack, info.Priority, info.Loop);
            }
            else return info.Name;

            loadedSounds[info.Name] = sound;
            return info.Name;
        }

        public void Tick()
        {
            soundSystem?.update();
        }

        public void Unload()
        {
            foreach (var channel in channels) channel.stop();
            foreach (var sound in loadedSounds.Values) sound.Dispose();
            foreach (var music in loadedMusic.Values) music.Dispose();
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
            var key = intro + loop;

            if (!string.IsNullOrEmpty(key) && loadedMusic.ContainsKey(intro + loop)) return loadedMusic[intro + loop];

            var music = new Music(soundSystem, intro, loop, volume);
            loadedMusic[intro + loop] = music;
            return music;
        }

        public void LoadMusicNSF(byte[] nsfData)
        {
            bgm = new BackgroundMusic(AudioContainer.LoadContainer(ref nsfData));
            AudioManager.Instance.LoadBackgroundMusic(bgm);
        }

        public void LoadSfxNSF(byte[] nsfData)
        {
            sfx = new SoundEffect(AudioContainer.LoadContainer(ref nsfData), 1);
            AudioManager.Instance.LoadSoundEffect(sfx);
        }

        public void PlayMusicNSF(uint track)
        {
            bgm.CurrentTrack = track - 1;
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
