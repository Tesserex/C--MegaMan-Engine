using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD;

namespace Mega_Man
{
    public class SoundEffect : IDisposable
    {
        private CHANNEL_CALLBACK callback;
        private Sound sound;
        private Channel channel = null;
        private FMOD.System system;
        private int playCount;

        public SoundEffect(FMOD.System system, string path, bool loop)
        {
            this.system = system;
            callback = new CHANNEL_CALLBACK(SyncCallback);

            system.createSound(path, MODE.SOFTWARE | (loop ? MODE.LOOP_NORMAL : MODE.LOOP_OFF), ref sound);
            channel = new Channel();
            playCount = 0;
        }

        public void Play()
        {
            channel.setCallback(null);
            channel.stop();   // restart sound
            system.playSound(CHANNELINDEX.FREE, sound, false, ref channel);
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
                channel.stop();
            }
        }

        public void StopIfLooping()
        {
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
}
