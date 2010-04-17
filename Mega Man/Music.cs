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

        public bool Playing { get; private set; }

        public Music(FMOD.System system, string intropath, string looppath)
        {
            RESULT result;
            this.system = system;
            callback = new CHANNEL_CALLBACK(SyncCallback);

            result = system.createSound(looppath, MODE.LOOP_NORMAL, ref loop);
            
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
                float vol = 0;
                if (channel != null)
                {
                    channel.getVolume(ref vol);
                }
                return vol;
            }
            set
            {
                if (channel != null) channel.setVolume(value);
            }
        }

        public void Play()
        {
            Playing = false;
            if (intro != null)
            {
                CHANNELINDEX index = (channel == null) ? CHANNELINDEX.FREE : CHANNELINDEX.REUSE;
                system.playSound(index, intro, false, ref channel);
                channel.setVolume(1);
                playingintro = true;
                channel.setCallback(callback);
            }
            else system.playSound(CHANNELINDEX.FREE, loop, false, ref channel);

            Playing = true;
        }

        public void Stop()
        {
            if (channel == null) return;
            Playing = false;
            channel.stop();
            channel.setPosition(0, TIMEUNIT.MS);
        }

        private FMOD.RESULT SyncCallback(IntPtr c, CHANNEL_CALLBACKTYPE type, IntPtr a, IntPtr b)
        {
            if (Playing && playingintro && type == CHANNEL_CALLBACKTYPE.END)
            {
                system.playSound(CHANNELINDEX.REUSE, loop, false, ref channel);
                channel.setVolume(1);
                playingintro = false;
            }

            return RESULT.OK;
        }
    }
}
