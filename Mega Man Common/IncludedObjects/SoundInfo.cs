
namespace MegaMan.Common.IncludedObjects
{
    public class SoundInfo : IncludedObject
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
