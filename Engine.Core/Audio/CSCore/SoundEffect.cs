using CSCore.Codecs.WAV;
using CSCore.SoundOut;
using MegaMan.Engine.Core.Audio;
using NSF4Net;

namespace MegaMan.Engine.Core.Audio.CSCore
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
        private int playCount;
        private readonly WasapiOut soundOut;
        private readonly WaveFileReader wav;
        private readonly float baseVolume;
        private float volume;
        private readonly bool loop;

        public WavEffect(string path, bool loop, float baseVol)
        {
            baseVolume = baseVol;
            volume = 1;
            this.loop = loop;

            playCount = 0;

            soundOut = new WasapiOut();
            wav = new WaveFileReader(path);
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
            }
        }

        public void Play()
        {
            playCount++;
        }

        public void Stop()
        {
            soundOut.Stop();
        }

        public void StopIfLooping()
        {
            if (loop)
            {
                Stop();
            }
        }

        public void Dispose()
        {
            soundOut.Dispose();
            wav.Dispose();
        }
    }

    public class NsfEffect : ISoundEffect
    {
        private readonly NsfPlayer nsfPlayer;
        private readonly int track;
        private readonly byte priority;
        private readonly bool loop;
        private bool playing;

        public NsfEffect(NsfPlayer nsfPlayer, int track, byte priority, bool loop)
        {
            this.nsfPlayer = nsfPlayer;
            this.track = track;
            this.priority = priority;
            this.loop = loop;
            playing = false;
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
            playing = true;
            nsfPlayer.PlaySfx(track);
        }

        public void Stop()
        {
            playing = false;
        }

        public void StopIfLooping() { if (loop) Stop(); }

        public void Dispose()
        {
            Stop();
        }
    }
}
