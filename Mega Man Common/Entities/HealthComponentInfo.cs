
namespace MegaMan.Common.Entities
{
    public class HealthComponentInfo : IComponentInfo
    {
        public float Max { get; set; }
        public float? StartValue { get; set; }
        public int FlashFrames { get; set; }

        public MeterInfo Meter { get; set; }
    }
}
