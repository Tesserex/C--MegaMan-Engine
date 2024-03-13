using CSCore;
using CSCore.SoundOut;

namespace Engine.Core.Audio.CSCore
{
    internal class CSCorePlayer : IAudioPlayer
    {
        private readonly WasapiOut soundOut;

        public CSCorePlayer(IWaveSource waveSource)
        {
            soundOut = new WasapiOut();
            soundOut.Initialize(waveSource);
        }

        public void Play()
        {
            soundOut.Play();
        }

        public void Stop()
        {
            soundOut.Stop();
        }
    }
}
