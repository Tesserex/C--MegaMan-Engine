
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

        public SoundInfo Clone()
        {
            return new SoundInfo {
                Name = Name,
                Path = Path.Clone(),
                NsfTrack = NsfTrack,
                Loop = Loop,
                Volume = Volume,
                Priority = Priority,
                Type = Type
            };
        }
    }
}
