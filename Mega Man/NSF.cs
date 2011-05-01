using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Mega_Man
{
    public class NSF : IDisposable
    {
		[DllImport("nsf.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int load_nsf_file(string filename, int track, int rate);

		[DllImport("nsf.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int process_buffer(short[] buffer, int bufsize);

		[DllImport("nsf.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_channel(int channel, bool enabled);

		[DllImport("nsf.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_song(int track);

		[DllImport("nsf.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int playback_rate();

		[DllImport("nsf.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void unload_nsf();

        private FMOD.System system;
        private FMOD.Sound sound = null;
        private FMOD.Channel channel = null;
        private FMOD.CREATESOUNDEXINFO createsoundexinfo = new FMOD.CREATESOUNDEXINFO();

        private static FMOD.MODE mode = (FMOD.MODE._2D | FMOD.MODE.DEFAULT | FMOD.MODE.OPENUSER | FMOD.MODE.LOOP_NORMAL | FMOD.MODE.HARDWARE);

        private FMOD.SOUND_PCMREADCALLBACK pcmreadcallback;

        private static uint frequency = 48000;
        private int len;

        private bool loaded = false;
        private bool sq1 = true, sq2 = true, tri = true, noise = true, dpcm = true;

        public bool Square1Enabled
        {
            get { return sq1; }
            set
            {
                sq1 = value;
                set_channel(0, value);
            }
        }

        public bool Square2Enabled
        {
            get { return sq2; }
            set
            {
                sq2 = value;
                set_channel(1, value);
            }
        }

        public bool TriangleEnabled
        {
            get { return tri; }
            set
            {
                tri = value;
                set_channel(2, value);
            }
        }

        public bool NoiseEnabled
        {
            get { return noise; }
            set
            {
                noise = value;
                set_channel(3, value);
            }
        }

        public bool DPCMEnabled
        {
            get { return dpcm; }
            set
            {
                dpcm = value;
                set_channel(4, value);
            }
        }

        private float baseVolume = 1, volume = 1;
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

        public void Load(string filename)
        {
            load_nsf_file(filename, 1, (int)frequency);
            loaded = true;

            int rate = playback_rate();
            if (rate == 0) throw new FileNotFoundException("The NSF was either not found or invalid.", filename);

            len = (int)frequency / rate;

            pcmreadcallback = new FMOD.SOUND_PCMREADCALLBACK(PCMREADCALLBACK);

            createsoundexinfo.cbsize = Marshal.SizeOf(createsoundexinfo);
            createsoundexinfo.fileoffset = 0;
            createsoundexinfo.decodebuffersize = (uint)len * 4;
            createsoundexinfo.length = (uint)len * 8;
            createsoundexinfo.numchannels = 1;
            createsoundexinfo.defaultfrequency = (int)frequency;
            createsoundexinfo.format = FMOD.SOUND_FORMAT.PCM16;
            createsoundexinfo.pcmreadcallback = pcmreadcallback;
            createsoundexinfo.dlsname = null;

            sq1 = sq2 = tri = noise = dpcm = true;

            if (sound != null) sound.release();

            system.createSound(
                (string)null,
                (mode | FMOD.MODE.CREATESTREAM),
                ref createsoundexinfo,
                ref sound);
        }

        public void SetTrack(int track)
        {
            Stop();
            if (frequency >= 0) set_song(track);
        }

        public NSF(FMOD.System system)
        {
            this.system = system;
        }

        public FMOD.RESULT Play()
        {
            return system.playSound(FMOD.CHANNELINDEX.FREE, sound, false, ref channel);
        }

        public void Stop()
        {
            if (channel != null)
            {
                channel.stop();
            }
        }

        public void FadeOut(int frames)
        {
            if (channel == null) return;

            float fadeamt = 1.0f / frames;
            Engine.Instance.DelayedCall(Stop, (i) => { Volume -= fadeamt; }, frames);
        }

        #region IDisposable
        ~NSF()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (channel != null) channel.stop();

                if (sound != null)
                {
                    sound.release();
                }
            }

            if (loaded) unload_nsf();
        }
        #endregion

        private short[] remain, buffer;
        private uint rem_len = 0;
        private FMOD.RESULT PCMREADCALLBACK(IntPtr soundraw, IntPtr data, uint datalen)
        {
            datalen /= 2;
            short[] full = new short[datalen];

            buffer = new short[len];

            uint index = 0;
            if (rem_len > 0)
            {
                for (int i = 0; i < len - rem_len; i++)
                {
                    if (i >= datalen)
                    {
                        break;
                    }
                    full[i] = remain[i + rem_len];
                }
                index += rem_len;
            }
            while (index <= datalen - len)
            {
                process_buffer(buffer, len);
                for (int i = 0; i < len; i++)
                {
                    full[index] = buffer[i];
                    index++;
                }
            }
            if (index < datalen)
            {
                remain = new short[len];
                process_buffer(remain, len);
                rem_len = datalen - index;
                for (int i = 0; i < rem_len; i++)
                {
                    full[index] = remain[i];
                    index++;
                }
            }
            else rem_len = 0;

            Marshal.Copy(full, 0, data, (int)datalen);

            return FMOD.RESULT.OK;
        }
    }
}
