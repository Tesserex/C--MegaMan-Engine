﻿using System;
using System.Collections.Generic;
using FMOD;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
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
                AudioManager.Instance.ChangeVolume(volume / 100f);
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
            set { if (bgm != null) AudioManager.Instance.MuteChannel(0, !value); }
        }

        public bool SquareTwo
        {
            get { return AudioManager.Instance.Muted[1]; }
            set { if (bgm != null) AudioManager.Instance.MuteChannel(1, !value); }
        }

        public bool Triangle
        {
            get { return AudioManager.Instance.Muted[2]; }
            set { if (bgm != null) AudioManager.Instance.MuteChannel(2, !value); }
        }

        public bool Noise
        {
            get { return AudioManager.Instance.Muted[3]; }
            set { if (bgm != null) AudioManager.Instance.MuteChannel(3, !value); }
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
            if (AudioManager.Instance.Paused)
                ApplyMusicSetting();
        }

        public void Stop()
        {
            updateTimer.Stop();
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
            else if (info.Type == AudioType.NSF)
            {
                sound = new NsfEffect(sfx, info.NsfTrack, info.Priority, info.Loop);
            }
            else return info.Name;

            loadedSounds[info.Name] = sound;
            return info.Name;
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
