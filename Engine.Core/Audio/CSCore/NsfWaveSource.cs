using CSCore;
using NSF4Net;

namespace MegaMan.Engine.Core.Audio.CSCore
{
    public class NsfWaveSource : IWaveSource
    {
        private const int CHANNELS = 2;
        private NsfPlayer player;
        private WaveFormat _waveFormat;

        private double volume = 1d;
        public double Volume { get { return volume; } set { volume = Math.Clamp(value, 0, 1); } }

        public NsfWaveSource(NsfPlayer player)
        {
            this.player = player;
            _waveFormat = new WaveFormat(player.SampleRate, 16, CHANNELS);
        }

        public bool CanSeek => false;

        public WaveFormat WaveFormat => _waveFormat;

        public long Position { get => 0; set { } }

        public long Length => 0;

        public void Dispose()
        {
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            for (var bufPos = 0; bufPos < count;)
            {
                double sample = player.TickSample();
                var output = (ushort)(256 * Volume * sample);
                for (var i = 0; i < CHANNELS; i++)
                {
                    buffer[bufPos++] = (byte)(output & 0xFF);
                    buffer[bufPos++] = (byte)(output >> 8);
                }
            }

            return count;
        }
    }
}
