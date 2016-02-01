
namespace MegaMan.Common
{
    public class SoundInfo
    {
        public string Name { get; set; }
        public FilePath Path { get; set; }
        public int NsfTrack { get; set; }
        public bool Loop { get; set; }
        public float Volume { get; set; }
        public byte Priority { get; set; }
        public AudioType Type { get; set; }
    }
}
