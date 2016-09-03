using MegaMan.Common.Geometry;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.Common
{
    public class MeterInfo : IHandlerObjectInfo
    {
        public enum Orientation : byte
        {
            Horizontal,
            Vertical
        }

        public string Name { get; set; }
        public PointF Position { get; set; }
        public FilePath Background { get; set; }
        public FilePath TickImage { get; set; }
        public Orientation Orient { get; set; }
        public Point TickOffset { get; set; }
        public SoundInfo Sound { get; set; }
        public SceneBindingInfo Binding { get; set; }
    }
}
