using System.Drawing;

namespace MegaMan.LevelEditor
{
    public class StartPositionTool : ITool
    {
        private static readonly Bitmap icon;

        static StartPositionTool()
        {
            icon = new Bitmap(21,24);
            icon.SetResolution(96, 96);
            using (Graphics g = Graphics.FromImage(icon))
            {
                g.DrawImage(Properties.Resources.start_full, 0, 0, 21, 24);
            }
        }

        public static Image MegaMan
        {
            get { return icon; }
        }

        #region ITool Members

        public Image Icon
        {
            get
            {
                return icon;
            }
        }
        public Point IconOffset { get { return new Point(0, -8); } }
        public bool IconSnap { get { return true; } }

        public bool IsIconCursor { get { return false; } }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            int px = (location.X / surface.Screen.Tileset.TileSize) * surface.Screen.Tileset.TileSize + 4;
            int py = (location.Y / surface.Screen.Tileset.TileSize) * surface.Screen.Tileset.TileSize + 4;

            surface.Screen.Stage.StartScreen = surface.Screen.Name;
            surface.Screen.Stage.StartPoint = new Point(px, py);
            surface.ReDrawEntities();
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
            
        }

        public void Release(ScreenDrawingSurface surface)
        {
            
        }

        public void RightClick(ScreenDrawingSurface surface, Point location)
        {

        }

        #endregion
    }
}