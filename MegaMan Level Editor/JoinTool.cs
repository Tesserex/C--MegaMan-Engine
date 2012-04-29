using System.Drawing;
using System.Windows.Forms;
using MegaMan.Common;

namespace MegaMan.LevelEditor
{
    public class JoinTool : ITool
    {
        public Image Icon { get { return Properties.Resources.joins; } }
        public Point IconOffset { get { return Point.Empty; } }
        public bool IconSnap { get { return false; } }
        public bool IsIconCursor { get { return true; } }

        private static Join NearestJoin(ScreenDrawingSurface surface, Point location)
        {
            Join nearest = null;

            foreach (var join in surface.Screen.Stage.Joins)
            {
                if (join.screenOne == surface.Screen.Name)
                {
                    int begin = join.offsetOne * surface.Screen.Tileset.TileSize;
                    int end = (join.offsetOne + join.Size) * surface.Screen.Tileset.TileSize;
                    if (join.type == JoinType.Vertical)
                    {
                        if (location.X > surface.Width - surface.Screen.Tileset.TileSize && location.Y >= begin && location.Y <= end)
                        {
                            nearest = join;
                        }
                    }
                    else
                    {
                        if (location.Y > surface.Height - surface.Screen.Tileset.TileSize && location.X >= begin && location.X <= end)
                        {
                            nearest = join;
                        }
                    }
                }
                else if (join.screenTwo == surface.Screen.Name)
                {
                    int begin = join.offsetTwo * surface.Screen.Tileset.TileSize;
                    int end = (join.offsetTwo + join.Size) * surface.Screen.Tileset.TileSize;
                    if (join.type == JoinType.Vertical)
                    {
                        if (location.X < surface.Screen.Tileset.TileSize && location.Y >= begin && location.Y <= end)
                        {
                            nearest = join;
                        }
                    }
                    else
                    {
                        if (location.Y < surface.Screen.Tileset.TileSize && location.X >= begin && location.X <= end)
                        {
                            nearest = join;
                        }
                    }
                }
            }
            return nearest;
        }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            ContextMenu menu = new ContextMenu();

            // find a join to modify
            var nearest = NearestJoin(surface, location);

            if (nearest != null)
            {
                string typeText = (nearest.type == JoinType.Vertical)? "Left-Right" : "Up-Down";

                menu.MenuItems.Add("Modify " + typeText + " Join from " + nearest.screenOne + " to " + nearest.screenTwo, (s, e) => EditJoin(surface, nearest));

                menu.MenuItems.Add("Delete " + typeText + " Join from " + nearest.screenOne + " to " + nearest.screenTwo, (s, e) => DeleteJoin(surface, nearest));
            }
            else
            {
                if (location.X > surface.Width - surface.Screen.Tileset.TileSize)
                {
                    menu.MenuItems.Add(new MenuItem("New Rightward Join from " + surface.Screen.Name,
                                                    (s, e) => NewJoin(surface, surface.Screen.Name, "", JoinType.Vertical, location.Y / surface.Screen.Tileset.TileSize)));
                }
                if (location.X < surface.Screen.Tileset.TileSize)
                {
                    menu.MenuItems.Add(new MenuItem("New Leftward Join from " + surface.Screen.Name,
                                                    (s, e) => NewJoin(surface, "", surface.Screen.Name, JoinType.Vertical, location.Y / surface.Screen.Tileset.TileSize)));
                }
                if (location.Y > surface.Height - surface.Screen.Tileset.TileSize)
                {
                    menu.MenuItems.Add(new MenuItem("New Downward Join from " + surface.Screen.Name,
                                                    (s, e) => NewJoin(surface, surface.Screen.Name, "", JoinType.Horizontal, location.X / surface.Screen.Tileset.TileSize)));
                }
                if (location.Y < surface.Screen.Tileset.TileSize)
                {
                    menu.MenuItems.Add(new MenuItem("New Upward Join from " + surface.Screen.Name,
                                                    (s, e) => NewJoin(surface, "", surface.Screen.Name, JoinType.Horizontal, location.X / surface.Screen.Tileset.TileSize)));
                }
            }
            menu.Show(surface, location);
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
        }

        public void Release(ScreenDrawingSurface surface)
        {
        }

        private static void NewJoin(ScreenDrawingSurface surface, string s1, string s2, JoinType type, int offset)
        {
            Join newjoin = new Join {screenTwo = s2, screenOne = s1, type = type, Size = 1};
            newjoin.offsetOne = newjoin.offsetTwo = offset;
            JoinForm form = new JoinForm(newjoin, surface.Screen.Stage.Screens);
            form.OK += () => surface.Screen.Stage.AddJoin(newjoin);
            form.Show();
        }

        private static void EditJoin(ScreenDrawingSurface surface, Join join)
        {
            JoinForm form = new JoinForm(join, surface.Screen.Stage.Screens);
            form.OK += () => surface.Screen.Stage.RaiseJoinChange(join);
            form.Show();
        }

        private static void DeleteJoin(ScreenDrawingSurface surface, Join join)
        {
            surface.Screen.Stage.RemoveJoin(join);
        }

        public void RightClick(ScreenDrawingSurface surface, Point location)
        {

        }
    }
}