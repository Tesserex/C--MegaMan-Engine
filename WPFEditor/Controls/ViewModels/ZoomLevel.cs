
namespace MegaMan.Editor.Controls.ViewModels
{
    public struct ZoomLevel
    {
        private double _zoom;
        public double Zoom { get { return _zoom; } }

        public override string ToString()
        {
            return Zoom.ToString("P0");
        }

        public static ZoomLevel Half
        {
            get
            {
                return new ZoomLevel() { _zoom = 0.5 };
            }
        }

        public static ZoomLevel Full
        {
            get
            {
                return new ZoomLevel() { _zoom = 1 };
            }
        }

        public static ZoomLevel Double
        {
            get
            {
                return new ZoomLevel() { _zoom = 2 };
            }
        }

        public static ZoomLevel Triple
        {
            get
            {
                return new ZoomLevel() { _zoom = 3 };
            }
        }
    }
}
