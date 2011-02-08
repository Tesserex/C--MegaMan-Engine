using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD;

namespace Mega_Man
{
    public class Music : IDisposable
    {
        private CHANNEL_CALLBACK callback;
        private Sound intro = null, loop = null;
        private Channel channel = null;
        private FMOD.System system;
        private bool playingintro = false;

        private float baseVolume, volume;

        public bool Playing { get; private set; }

        private int nsfTrack;

        public Music(FMOD.System system, string intropath, string looppath, float baseVol, int nsfTrack)
        {
            RESULT result;
            this.system = system;
            callback = new CHANNEL_CALLBACK(SyncCallback);

            baseVolume = baseVol;
            volume = 1;
            this.nsfTrack = nsfTrack;

            if (looppath != null) result = system.createSound(looppath, MODE.LOOP_NORMAL, ref loop);
            
            if (intropath != null)
            {
                result = system.createSound(intropath, MODE.DEFAULT, ref intro);
            }

            Playing = false;
        }

        public void Dispose()
        {
            if (intro != null) intro.release();
            if (loop != null) loop.release();
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
                if (this.nsfTrack > 0) Engine.Instance.SoundSystem.NsfMusic.Volume = volume;
            }
        }

        public void Play()
        {
            Playing = false;
            if (intro != null)
            {
                CHANNELINDEX index = (channel == null) ? CHANNELINDEX.FREE : CHANNELINDEX.REUSE;
                system.playSound(index, intro, false, ref channel);
                Volume = 1;
                playingintro = true;
                channel.setCallback(callback);
            }
            else if (loop != null) system.playSound(CHANNELINDEX.FREE, loop, false, ref channel);

            if (nsfTrack > 0) Engine.Instance.SoundSystem.PlayTrack(nsfTrack);

            Playing = true;
        }

        public void Stop()
        {
            if (channel != null)
            {
                channel.stop();
                channel.setPosition(0, TIMEUNIT.MS);
            }
            if (nsfTrack > 0) Engine.Instance.SoundSystem.StopNSF();
            Playing = false;
        }

        public void FadeOut(int frames)
        {
            if (!Playing) return;

            if (channel != null)
            {
                float fadeamt = 1.0f / frames;
                Engine.Instance.DelayedCall(Stop, (i) => { Volume -= fadeamt; }, frames);
            }
            if (nsfTrack > 0) Engine.Instance.SoundSystem.NsfMusic.FadeOut(frames);
        }

        private FMOD.RESULT SyncCallback(IntPtr c, CHANNEL_CALLBACKTYPE type, IntPtr a, IntPtr b)
        {
            if (Playing && playingintro && type == CHANNEL_CALLBACKTYPE.END)
            {
                system.playSound(CHANNELINDEX.REUSE, loop, false, ref channel);
                channel.setVolume(volume);
                playingintro = false;
            }

            return RESULT.OK;
        }
    }
}
