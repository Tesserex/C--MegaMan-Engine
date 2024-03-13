using System;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.SoundOut;

namespace Engine.Core.Audio.CSCore
{
    public class MusicWav : IAudioObject, IDisposable
    {
        private bool playingintro;

        private WaveFileReader? intro;
        private WaveFileReader? loop;
        private WasapiOut soundOut;

        public bool Playing { get; private set; }

        public MusicWav(string? intropath, string? looppath)
        {
            soundOut = new WasapiOut();
            soundOut.Stopped += (s, e) => {
                if (playingintro && loop != null)
                {
                    playingintro = false;
                    soundOut.Initialize(loop);
                    soundOut.Play();
                }
            };

            if (intropath != null)
            {
                intro = new WaveFileReader(intropath);
            }
            if (looppath != null)
            {
                loop = new WaveFileReader(looppath);
            }

            Playing = false;
        }

        public void Play()
        {
            Playing = false;
            if (intro != null)
            {
                playingintro = true;
                soundOut.Initialize(intro);
                soundOut.Play();
            }
            else if (loop != null)
            {
                playingintro = false;
                soundOut.Initialize(loop);
                soundOut.Play();
            }

            Playing = true;
        }

        public void Stop()
        {
            Playing = false;
            playingintro = false;
        }

        public void Dispose()
        {
            soundOut.Dispose();
            intro?.Dispose();
            loop?.Dispose();
        }
    }
}
