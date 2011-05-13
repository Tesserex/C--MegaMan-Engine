using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD;
using System.Xml.Linq;
using MegaManR.Audio;

namespace Mega_Man
{
    public interface ISoundEffect : IDisposable
    {
        float Volume { get; set; }
        void Play();
        void Stop();
        void StopIfLooping();
    }

    public class WavEffect : ISoundEffect
    {
        private CHANNEL_CALLBACK callback;
        private Sound sound;
        private Channel channel = null;
        private FMOD.System system;
        private int playCount;

        private float baseVolume, volume;

        public WavEffect(FMOD.System system, string path, bool loop, float baseVol)
        {
            this.system = system;
            callback = new CHANNEL_CALLBACK(SyncCallback);

            baseVolume = baseVol;
            volume = 1;

            system.createSound(path, MODE.SOFTWARE | (loop ? MODE.LOOP_NORMAL : MODE.LOOP_OFF), ref sound);
            channel = new Channel();
            playCount = 0;
        }

        public float Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = baseVolume * value;
                if (channel != null) channel.setVolume(volume);
            }
        }

        public void Play()
        {
            channel.setCallback(null);
            channel.stop();   // restart sound
            system.playSound(CHANNELINDEX.FREE, sound, false, ref channel);
            channel.setVolume(volume);
            channel.setCallback(callback);
            playCount++;
        }

        public void Stop()
        {
            if (playCount == 0) return;
            playCount--;
            if (playCount <= 0)
            {
                playCount = 0;
                if (channel != null) channel.stop();
            }
        }

        public void StopIfLooping()
        {
            if (sound == null) return;
            MODE mode = MODE.DEFAULT;
            sound.getMode(ref mode);
            if ((mode & MODE.LOOP_NORMAL) == MODE.LOOP_NORMAL)
            {
                Stop();
            }
        }

        public void Dispose()
        {
            if (sound != null) sound.release();
        }

        private RESULT SyncCallback(IntPtr c, CHANNEL_CALLBACKTYPE type, IntPtr a, IntPtr b)
        {
            if (type == CHANNEL_CALLBACKTYPE.END)
            {
                playCount--;
                if (playCount < 0) playCount = 0;
            }
            return RESULT.OK;
        }
    }

    public class NsfEffect : ISoundEffect
    {
        private int track;
        private SoundEffect sfx;
        private byte priority;
        private bool loop;
        private bool playing;

        public NsfEffect(SoundEffect fx, int track, byte priority, bool loop)
        {
            this.track = track - 1;
            this.sfx = fx;
            this.priority = priority;
            this.loop = loop;
            this.playing = false;
        }

        public float Volume
        {
            get
            {
                return 1f;
            }
            set
            {
                
            }
        }

        public void Play()
        {
            if (this.priority > SoundSystem.CurrentSfxPriority) return;

            if (loop && !playing) AudioManager.Instance.SFXPlaybackStopped += PlayOnce;
            playing = true;
            PlayOnce();
        }

        private void PlayOnce()
        {
            if (this.priority > SoundSystem.CurrentSfxPriority)
            {
                Stop();
                return;
            }

            SoundSystem.CurrentSfxPriority = this.priority;
            this.sfx.CurrentTrack = (uint)this.track;
            this.sfx.Priority = this.priority;
            AudioManager.Instance.PlaySoundEffect(this.sfx);
        }

        public void Stop()
        {
            if (this.sfx.CurrentTrack == this.track) AudioManager.Instance.StopSFXPlayback();
            if (loop) AudioManager.Instance.SFXPlaybackStopped -= PlayOnce;
            playing = false;
        }

        public void StopIfLooping() { if (loop) Stop(); }

        public void Dispose()
        {
            Stop();
        }
    }
}
