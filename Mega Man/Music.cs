using System;
using FMOD;

namespace Mega_Man
{
    public class Music : IDisposable
    {
        private readonly CHANNEL_CALLBACK callback;
        private readonly Sound intro;
        private readonly Sound loop;
        private Channel channel;
        private readonly FMOD.System system;
        private bool playingintro;

        private readonly float baseVolume;
        private float volume;

        public bool Playing { get; private set; }

        public Music(FMOD.System system, string intropath, string looppath, float baseVol)
        {
            this.system = system;
            callback = new CHANNEL_CALLBACK(SyncCallback);

            baseVolume = baseVol;
            volume = 1;

            if (looppath != null) system.createSound(looppath, MODE.LOOP_NORMAL, ref loop);
            
            if (intropath != null)
            {
                system.createSound(intropath, MODE.DEFAULT, ref intro);
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

            Playing = true;
        }

        public void Stop()
        {
            Playing = false;
            if (channel != null)
            {
                channel.stop();
                channel.setPosition(0, TIMEUNIT.MS);
            }
        }

        public void FadeOut(int frames)
        {
            if (!Playing) return;

            if (channel != null)
            {
                float fadeamt = 1.0f / frames;
                Engine.Instance.DelayedCall(Stop, i => { Volume -= fadeamt; }, frames);
            }
        }

        private RESULT SyncCallback(IntPtr c, CHANNEL_CALLBACKTYPE type, IntPtr a, IntPtr b)
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
