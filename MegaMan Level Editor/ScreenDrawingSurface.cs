using System;
using System.Drawing;
using System.Windows.Forms;
using MegaMan.Common;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MegaMan.LevelEditor
{
    public class ScreenEditEventArgs : EventArgs
    {
        public HistoryAction Action { get; private set; }

        public ScreenEditEventArgs(HistoryAction action)
        {
            Action = action;
        }
    }

    /* *
     * ScreenDrawingSurface - Draw a screen onto one of these. 
     * Multiple screen surfaces show an entire map in one window
     * */
    public class ScreenDrawingSurface : PictureBox
    {
        private static readonly Brush blockBrush = new SolidBrush(Color.FromArgb(160, Color.OrangeRed));
        private static readonly Brush ladderBrush = new SolidBrush(Color.FromArgb(160, Color.Yellow));
        private static readonly Pen passPen = new Pen(Color.Blue, 4);
        private static readonly Pen blockPen = new Pen(Color.Red, 4);
        private static readonly Pen gridPen = new Pen(new SolidBrush(Color.FromArgb(160, Color.YellowGreen)));

        private Bitmap tileLayer;
        private Bitmap gridLayer;
        private Bitmap blockLayer;
        private Bitmap entityLayer;
        private Bitmap mouseLayer;
        private Bitmap joinLayer;
        private Bitmap toolLayer;
        private Bitmap masterImage;
        private Bitmap grayTiles;

        private bool grayDirty;

        private double zoomFactor = 1;

        private Pen selectionPen;
        public Rectangle? Selection { get; private set; }
        public event Action<ScreenDrawingSurface, Rectangle?> SelectionChanged;

        private static readonly ColorMatrix grayMatrix = new ColorMatrix(
           new float[][] 
          {
             new float[] {.3f, .3f, .3f, 0, 0},
             new float[] {.59f, .59f, .59f, 0, 0},
             new float[] {.11f, .11f, .11f, 0, 0},
             new float[] {0, 0, 0, 1, 0},
             new float[] {0, 0, 0, 0, 1}
          });

        private bool active;
        public bool Placed { get; set; }

        public ScreenDocument Screen { get; private set; }

        public event EventHandler<ScreenEditEventArgs> Edited;
        public event Action Activated;

        #region Constructors

        public ScreenDrawingSurface(ScreenDocument screen)
        {
            Screen = screen;
            SizeMode = PictureBoxSizeMode.StretchImage;

            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.None;

            BuildLayers();

            Screen.Resized += (w, h) => ResizeLayers();

            Program.AnimateTick += Animate;
            Program.FrameTick += SelectionAnimate;

            RedrawJoins();
            ReDrawAll();

            MainForm.Instance.DrawOptionToggled += ReDrawMaster;

            selectionPen = new Pen(Color.LimeGreen, 2);
            selectionPen.DashPattern = new float[] { 3, 2 };
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                selectionPen.Dispose();
                Program.AnimateTick -= Animate;
                Program.FrameTick -= SelectionAnimate;
            }
        }

        #endregion

        public void EditedWithAction(HistoryAction action)
        {
            if (Edited != null) Edited(this, new ScreenEditEventArgs(action));
        }

        private void Animate()
        {
            if (active)
            {
                ReDrawTiles();
                ReDrawEntities();
                ReDrawMaster();
            }
        }

        private int tickFrame = 0;
        private void SelectionAnimate()
        {
            if (Selection != null)
            {
                tickFrame++;
                if (tickFrame >= 5)
                {
                    selectionPen.DashOffset++;
                    if (selectionPen.DashOffset > 4) selectionPen.DashOffset = 0;
                    tickFrame = 0;
                    DrawSelectionAnts();
                }
            }
        }

        #region Mouse Handlers

        protected override void OnMouseEnter(EventArgs e)
        {
            active = true;
            if (Activated != null) Activated();

            if (MainForm.Instance.CurrentTool != null)
            {
                var tool = MainForm.Instance.CurrentTool;
                if (tool.IsIconCursor)
                {
                    Cursor = CreateCursor((Bitmap)tool.Icon, tool.IconOffset.X, tool.IconOffset.Y);
                }
                else if (tool.Icon != null)
                {
                    Cursor.Hide();
                }
            }
            ReDrawMaster();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            active = false;

            Cursor = Cursors.Default;
            if (MainForm.Instance.CurrentTool != null)
            {
                var tool = MainForm.Instance.CurrentTool;
                if (!tool.IsIconCursor && tool.Icon != null)
                {
                    Cursor.Show();
                }
            }

            if (mouseLayer != null)
            {
                using (Graphics g = Graphics.FromImage(mouseLayer))
                {
                    g.Clear(Color.Transparent);
                }
            }
            ReDrawMaster();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            
            if (MainForm.Instance.CurrentTool == null) return;

            if (e.Button == MouseButtons.Left)
            {
                MainForm.Instance.CurrentTool.Click(this, MouseLocation(e));
            }
            else if (e.Button == MouseButtons.Right)
            {
                MainForm.Instance.CurrentTool.RightClick(this, MouseLocation(e));
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (MainForm.Instance.CurrentTool == null) return;

            if (e.Button == MouseButtons.Left)
            {
                MainForm.Instance.CurrentTool.Release(this);
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mouseLayer == null) return;

            if (MainForm.Instance.CurrentTool != null)
            {
                if (MainForm.Instance.CurrentTool.Icon != null && !MainForm.Instance.CurrentTool.IsIconCursor)
                {
                    var mouse = IconLocation(e);

                    Bitmap icon = (Bitmap)MainForm.Instance.CurrentTool.Icon;

                    icon.SetResolution(mouseLayer.HorizontalResolution, mouseLayer.VerticalResolution);

                    using (Graphics g = Graphics.FromImage(mouseLayer))
                    {
                        g.Clear(Color.Transparent);
                        g.DrawImageUnscaled(icon, mouse.X, mouse.Y, icon.Width, icon.Height);
                    }

                    ReDrawMaster();
                }

                if (e.Button == MouseButtons.Left)
                {
                    MainForm.Instance.CurrentTool.Move(this, new Point((int)(e.Location.X / zoomFactor), (int)(e.Location.Y / zoomFactor)));
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
        }

        public void Unfocus()
        {
            OnLostFocus(EventArgs.Empty);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Screen.SelectEntity(-1);
            ReDrawEntities();
            SetSelection(0, 0, 0, 0);
        }

        private Point IconLocation(MouseEventArgs e)
        {
            var mouse = MouseLocation(e);

            Point offset = MainForm.Instance.CurrentTool.IconOffset;

            return new Point(mouse.X + offset.X, mouse.Y + offset.Y);
        }

        private Point MouseLocation(MouseEventArgs e)
        {
            int tx = e.X;
            int ty = e.Y;

            Bitmap icon = (Bitmap)MainForm.Instance.CurrentTool.Icon;

            tx = (int)(tx / zoomFactor);
            ty = (int)(ty / zoomFactor);

            if (MainForm.Instance.CurrentTool.IconSnap)
            {
                tx = (tx / Screen.Tileset.TileSize) * Screen.Tileset.TileSize;
                ty = (ty / Screen.Tileset.TileSize) * Screen.Tileset.TileSize;
            }

            return new Point(tx, ty);
        }

        #endregion

        #region Layer Drawing

        public void RedrawJoins()
        {
            if (joinLayer == null) return;
            using (Graphics g = Graphics.FromImage(joinLayer))
            {
                g.Clear(Color.Transparent);
            }

            foreach (Join join in Screen.Stage.Joins)
            {
                if (join.screenOne == Screen.Name) DrawJoinEnd(join, true);
                else if (join.screenTwo == Screen.Name) DrawJoinEnd(join, false);
            }
            ReDrawMaster();
        }

        private void DrawJoinEnd(Join join, bool one)
        {
            if (joinLayer == null) return;
            using (Graphics g = Graphics.FromImage(joinLayer))
            {
                int offset = one ? join.offsetOne : join.offsetTwo;
                int start = offset * Screen.Tileset.TileSize;
                int end = start + (join.Size * Screen.Tileset.TileSize);
                int edge;
                Pen pen;

                if (one ? join.direction == JoinDirection.BackwardOnly : join.direction == JoinDirection.ForwardOnly) pen = blockPen;
                else pen = passPen;

                if (join.type == JoinType.Horizontal)
                {
                    edge = one ? Screen.PixelHeight - 2 : 2;
                    int curl = one ? edge - 6 : edge + 6;
                    g.DrawLine(pen, start, edge, end, edge);
                    g.DrawLine(pen, start + 1, edge, start + 1, curl);
                    g.DrawLine(pen, end - 1, edge, end - 1, curl);
                }
                else
                {
                    edge = one ? Screen.PixelWidth - 2 : 2;
                    int curl = one ? edge - 6 : edge + 6;
                    g.DrawLine(pen, edge, start, edge, end);
                    g.DrawLine(pen, edge, start, curl, start);
                    g.DrawLine(pen, edge, end, curl, end);
                }
            }
        }

        private void ReDrawAll()
        {
            ReDrawTiles();
            ReDrawEntities();
            ReDrawBlocking();
            ReDrawMaster();
            ReDrawGrid();
        }

        public void ReDrawTiles()
        {
            using (Graphics g = Graphics.FromImage(tileLayer))
            {
                Screen.DrawOn(g);
            }
            grayDirty = true;

            ReDrawMaster();
        }

        private void DrawGray()
        {
            if (!grayDirty) return;

            if (grayTiles != null) grayTiles.Dispose();
            grayTiles = ConvertToGrayscale(tileLayer);
            grayDirty = false;
        }

        private static Bitmap ConvertToGrayscale(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(grayMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        public void ReDrawEntities()
        {
            using (Graphics g = Graphics.FromImage(entityLayer))
            {
                g.Clear(Color.Transparent);

                Screen.DrawEntities(g);
                if (Screen.Name == Screen.Stage.StartScreen)
                {
                    g.DrawImage(StartPositionTool.MegaMan, Screen.Stage.StartPoint.X - 4, Screen.Stage.StartPoint.Y - 12);
                }
            }

            ReDrawMaster();
        }

        private void ReDrawBlocking()
        {
            using (Graphics g = Graphics.FromImage(blockLayer))
            {
                for (int y = 0; y < Screen.Height; y++)
                {
                    for (int x = 0; x < Screen.Width; x++)
                    {
                        if (Screen.TileAt(x, y).Properties.Blocking)
                        {
                            g.FillRectangle(blockBrush, x * Screen.Tileset.TileSize, y * Screen.Tileset.TileSize, Screen.Tileset.TileSize, Screen.Tileset.TileSize);
                        }

                        if (Screen.TileAt(x, y).Properties.Climbable)
                        {
                            g.FillRectangle(ladderBrush, x * Screen.Tileset.TileSize, y * Screen.Tileset.TileSize, Screen.Tileset.TileSize, Screen.Tileset.TileSize);
                        }
                    }
                }
            }
        }

        private void ReDrawGrid()
        {
            using (Graphics g = Graphics.FromImage(gridLayer))
            {
                for (int x = 0; x < Screen.Width; x++)
                {
                    int tx = x * Screen.Tileset.TileSize;
                    g.DrawLine(gridPen, tx, 0, tx, Screen.PixelHeight);
                }

                for (int y = 0; y < Screen.Height; y++)
                {
                    int ty = y * Screen.Tileset.TileSize;
                    g.DrawLine(gridPen, 0, ty, Screen.PixelWidth, ty);
                }
            }
        }

        private void ReDrawMaster()
        {
            if (Screen == null)
                return;

            using (Graphics g = Graphics.FromImage(masterImage))
            {
                g.Clear(Color.Black);

                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                if (MainForm.Instance.DrawTiles)
                {
                    if (active)
                    {
                        if (tileLayer != null) g.DrawImageUnscaled(tileLayer, 0, 0);
                    }
                    else
                    {
                        if (grayDirty) DrawGray();
                        if (grayTiles != null) g.DrawImageUnscaled(grayTiles, 0, 0);
                    }
                }

                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                if (MainForm.Instance.DrawEntities && entityLayer != null)
                    g.DrawImageUnscaled(entityLayer, 0, 0);

                if (MainForm.Instance.DrawBlock && blockLayer != null)
                    g.DrawImageUnscaled(blockLayer, 0, 0);

                if (MainForm.Instance.DrawGrid && gridLayer != null)
                    g.DrawImageUnscaled(gridLayer, 0, 0);

                if (MainForm.Instance.DrawJoins && joinLayer != null)
                    g.DrawImageUnscaled(joinLayer, 0, 0);

                if (toolLayer != null)
                    g.DrawImageUnscaled(toolLayer, 0, 0);

                if (active) g.DrawImageUnscaled(mouseLayer, 0, 0);
            }

            RefreshSize();
        }

        #endregion

        public Graphics GetToolLayerGraphics()
        {
            if (toolLayer == null) return null;
            return Graphics.FromImage(toolLayer);
        }

        public void ReturnToolLayerGraphics(Graphics g)
        {
            g.Dispose();
            ReDrawMaster();
        }

        public void Zoom(double factor)
        {
            zoomFactor = factor;
            RefreshSize();
        }

        public void SetSelection(int tx, int ty, int width, int height)
        {
            if (width != 0 && height != 0)
            {
                if (tx < 0)
                {
                    width += tx;
                    tx = 0;
                }
                if (ty < 0)
                {
                    height += ty;
                    ty = 0;
                }

                if (width + tx > Screen.Width) width = Screen.Width - tx;
                if (height + ty > Screen.Height) height = Screen.Height - ty;

                // all in tile sizes
                Selection = new Rectangle(tx, ty, width, height);

                DrawSelectionAnts();
            }
            else
            {
                Selection = null;

                using (Graphics g = Graphics.FromImage(toolLayer))
                {
                    g.Clear(Color.Transparent);
                }
            }

            if (SelectionChanged != null)
            {
                SelectionChanged(this, Selection);
            }
        }

        private void RefreshSize()
        {
            Width = (int)(masterImage.Width * zoomFactor);
            Height = (int)(masterImage.Height * zoomFactor);
            if (zoomFactor == 1)
            {
                Image = masterImage;
            }
            else
            {
                var img = new Bitmap((int)(masterImage.Width * zoomFactor), (int)(masterImage.Height * zoomFactor));
                using (var g = Graphics.FromImage(img))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(masterImage, 0, 0, img.Width, img.Height);
                }
                Image = img;
            }
            Refresh();
        }

        private void DrawSelectionAnts()
        {
            if (Selection == null) return;

            var size = Screen.Tileset.TileSize;

            var g = GetToolLayerGraphics();
            if (g != null)
            {
                g.Clear(Color.Transparent);

                // draw selection preview
                g.DrawRectangle(selectionPen,
                    new Rectangle(
                        Selection.Value.X * Screen.Tileset.TileSize,
                        Selection.Value.Y * Screen.Tileset.TileSize,
                        Selection.Value.Width * Screen.Tileset.TileSize,
                        Selection.Value.Height * Screen.Tileset.TileSize
                    )
                );

                ReturnToolLayerGraphics(g);
            }
        }

        #region Layer Helpers

        private void InitLayer(ref Bitmap layer)
        {
            if (layer != null) layer.Dispose();
            ResizeLayer(ref layer);
        }

        private void ResizeLayer(ref Bitmap layer)
        {
            if (layer != null) layer.Dispose();
            layer = new Bitmap(Screen.Width * Screen.Tileset.TileSize, Screen.Height * Screen.Tileset.TileSize);
        }

        private void BuildLayers()
        {
            InitLayer(ref tileLayer);
            InitLayer(ref grayTiles);
            InitLayer(ref gridLayer);
            InitLayer(ref blockLayer);
            InitLayer(ref entityLayer);
            InitLayer(ref mouseLayer);
            InitLayer(ref joinLayer);
            InitLayer(ref toolLayer);
            InitLayer(ref masterImage);
        }

        private void ResizeLayers()
        {
            ResizeLayer(ref tileLayer);
            ResizeLayer(ref grayTiles);
            ResizeLayer(ref gridLayer);
            ResizeLayer(ref entityLayer);
            ResizeLayer(ref blockLayer);
            ResizeLayer(ref mouseLayer);
            ResizeLayer(ref joinLayer);
            ResizeLayer(ref toolLayer);
            ResizeLayer(ref masterImage);
            ReDrawAll();
        }

        #endregion

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr ptr = bmp.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;
            ptr = CreateIconIndirect(ref tmp);
            return new Cursor(ptr);
        }
    }

    public struct IconInfo
    {
        public bool fIcon;
        public int xHotspot;
        public int yHotspot;
        public IntPtr hbmMask;
        public IntPtr hbmColor;
    }
}
