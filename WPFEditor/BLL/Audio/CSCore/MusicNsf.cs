using NSF4Net;

namespace MegaMan.Editor.Bll.Audio.CSCore
{
    public class MusicNsf : IAudioObject
    {
        private readonly NsfPlayer player;
        private readonly int track;

        public bool IsPlaying
        {
            get { return player.Playing; }
        }

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
