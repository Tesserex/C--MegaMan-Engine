namespace MegaMan.Common.Entities.Effects
{
    public class SpawnEffectPartInfo : IEffectPartInfo
    {
        public string Name { get; set; }
        public string State { get; set; }
        public PositionEffectPartInfo Position { get; set; }

        public IEffectPartInfo Clone()
        {
            return new SpawnEffectPartInfo {
                Name = Name,
                State = State,
                Position = (PositionEffectPartInfo)Position.Clone()
            };
        }
    }
}
