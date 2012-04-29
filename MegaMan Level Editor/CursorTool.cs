using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MegaMan.Common;
using System.Windows.Forms;

namespace MegaMan.LevelEditor
{
    public class CursorTool : ITool
    {
        private EntityPlacement heldEntity;
        private Point entityAnchor;

        public System.Drawing.Image Icon
        {
            get { return null; }
        }

        public bool IconSnap
        {
            get { return false; }
        }

        public bool IsIconCursor
        {
            get { return false; }
        }

        public void Click(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
            // select nearest entity
            var index = surface.Screen.FindEntityAt(location);
            surface.Screen.SelectEntity(index);
            surface.ReDrawEntities();

            heldEntity = surface.Screen.GetEntity(index);
            if (heldEntity != null)
            {
                entityAnchor = new Point(location.X - (int)heldEntity.screenX, location.Y - (int)heldEntity.screenY);
            }
            else
            {
                entityAnchor = Point.Empty;
            }
        }

        public void Move(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
            if (heldEntity != null)
            {
                heldEntity.screenX = location.X - entityAnchor.X;
                heldEntity.screenY = location.Y - entityAnchor.Y;
                surface.ReDrawEntities();
            }
        }

        public void Release(ScreenDrawingSurface surface)
        {
            
        }

        public void RightClick(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
            Click(surface, location);

            ContextMenuStrip menu = new ContextMenuStrip();

            if (heldEntity != null)
            {
                var deleteMenu = new ToolStripMenuItem(
                    String.Format("Delete {0}", heldEntity.entity),
                    Properties.Resources.Remove,
                    (s, e) =>
                    {
                        surface.Screen.RemoveEntity(heldEntity);
                        surface.ReDrawEntities();
                    }
                );

                menu.Items.Add(deleteMenu);
                menu.Items.Add(new ToolStripSeparator()); 
            }

            var testMenu = new ToolStripMenuItem(
                "Test From Here",
                Properties.Resources.protoman,
                (s, e) =>
                {
                    MainForm.Instance.RunTestFromStage(surface.Screen, location);
                }
            );

            menu.Items.Add(testMenu);

            menu.Show(surface, location);
        }

        public System.Drawing.Point IconOffset
        {
            get { return Point.Empty; }
        }
    }
}
