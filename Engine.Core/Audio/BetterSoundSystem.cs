using Engine.Core.Audio.CSCore;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using NSF4Net;

namespace Engine.Core.Audio
{
    internal class BetterSoundSystem : ISoundSystem
    {
        private IAudioPlayer player;
        private NsfPlayer nsfPlayer;
        private NsfWaveSource nsfWaveSource;

        private int volume;
        private bool musicEnabled;
        private bool sfxEnabled;

        private readonly Dictionary<string, MusicWav> loadedMusic = new Dictionary<string, MusicWav>();
        private readonly Dictionary<string, ISoundEffect> loadedSounds = new Dictionary<string, ISoundEffect>();

        public BetterSoundSystem()
        {
            nsfPlayer = new NsfPlayer(48000);
            nsfWaveSource = new NsfWaveSource(nsfPlayer);
            player = new CSCorePlayer(nsfWaveSource);
        }

        public bool MusicEnabled { get => musicEnabled; set { musicEnabled = value; if (!value) player.Stop(); } }
        public bool SfxEnabled { get; set; }
        public bool SquareOne { get => nsfPlayer.Square1Enabled; set { nsfPlayer.Square1Enabled = value; } }
        public bool SquareTwo { get => nsfPlayer.Square2Enabled; set { nsfPlayer.Square2Enabled = value; } }
        public bool Triangle { get => nsfPlayer.TriangleEnabled; set { nsfPlayer.TriangleEnabled = value; } }
        public bool Noise { get => nsfPlayer.NoiseEnabled; set { nsfPlayer.NoiseEnabled = value; } }
        public int Volume
        {
            get => volume;
            set
            {
                volume = value;
                nsfWaveSource.Volume = value / 256.0;
            }
        }

        public string EffectFromInfo(SoundInfo info)
        {
            if (loadedSounds.ContainsKey(info.Name)) return info.Name;

            ISoundEffect sound;
            if (info.Type == AudioType.Wav)
            {
                sound = new WavEffect(info.Path.Absolute, info.Loop, info.Volume);
            }
            else if (info.Type == AudioType.Nsf)
            {
                sound = new NsfEffect(nsfPlayer, info.NsfTrack, info.Priority, info.Loop);
            }
            else return info.Name;

            loadedSounds[info.Name] = sound;
            return info.Name;
        }

        public void LoadEffectsFromInfo(IEnumerable<SoundInfo> sounds)
        {
            foreach (var sound in sounds)
            {
                EffectFromInfo(sound);
            }
        }

        public IAudioObject LoadMusicWav(string? intro, string? loop)
        {
            var key = intro + loop;

            if (!string.IsNullOrEmpty(key) && loadedMusic.ContainsKey(key)) return loadedMusic[key];

            var music = new MusicWav(intro, loop);
            loadedMusic[key] = music;
            return music;
        }

        public void LoadNSF(byte[] nsfData)
        {
            using (var nsfStream = new MemoryStream(nsfData))
                nsfPlayer.LoadNsf(nsfStream);
        }

        public IAudioObject LoadMusicNsf(int track)
        {
            return new MusicNsf(nsfPlayer, track);
        }

        public void PlaySfx(string name)
        {
            if (!SfxEnabled) return;

            if (loadedSounds.ContainsKey(name))
            {
                loadedSounds[name].Play();
            }
        }

        public void Start()
        {
            player.Play();
        }

        public void Pause()
        {
            player.Stop();
        }

        public void StopMusicNsf()
        {
            nsfPlayer.Playing = false;
        }

        public void StopSfx(string name)
        {
        }

        public void StopSfxIfLooping(string name)
        {
        }

        public void Tick(float dt)
        {
        }

        public void Unload()
        {
            foreach (var sound in loadedSounds.Values) sound.Dispose();
            foreach (var music in loadedMusic.Values) music.Dispose();
            loadedSounds.Clear();
            loadedMusic.Clear();
        }
    }
}
