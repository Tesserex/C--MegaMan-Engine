using NSF4Net;

namespace Engine.Core.Audio.CSCore
{
    public class MusicNsf : IAudioObject
    {
        private readonly NsfPlayer player;
        private readonly int track;

        public MusicNsf(NsfPlayer player, int track)
        {
            this.player = player;
            this.track = track;
        }

        public void Play()
        {
            player.SelectSong(track);
            player.Playing = true;
        }

        public void Stop()
        {
            player.Playing = false;
        }
    }
}
