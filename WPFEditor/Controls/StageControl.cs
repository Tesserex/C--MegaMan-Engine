using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Bll;
using System.Windows.Media;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for StageLayoutControl.xaml
    /// </summary>
    public abstract class StageControl : UserControl
    {
        internal ScrollViewer scrollContainer;

        internal GridCanvas canvas;

        private IStageSelector _stageSelector;
        private StageDocument _stage;

        public StageDocument Stage
        {
            get
            {
                return _stage;
            }
            private set
            {
                if (_stage != null)
                {
                    Unhook();
                    ResetScreens();
                }

                _stage = value;

                if (_stage != null)
                {
                    Hook();
                    ResetScreens();
                }

                InvalidateVisual();
            }
        }

        public void SetStageSelector(IStageSelector selector)
        {
            if (_stageSelector != null)
            {
                _stageSelector.StageChanged -= StageChanged;
            }

            _stageSelector = selector;

            _stageSelector.StageChanged += StageChanged;

            Stage = selector.Stage;
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            Stage = e.Stage;
        }

        protected Dictionary<string, ScreenCanvas> _screens;
        protected bool _freezeLayout;

        private HashSet<string> _screensPlaced;
        private Size _stageSize;

        public StageControl()
        {
            InitializeComponent();

            _screens = new Dictionary<string, ScreenCanvas>();
            _screensPlaced = new HashSet<string>();

            this.SizeChanged += StageLayoutControl_SizeChanged;
        }

        public void InitializeComponent()
        {
            Background = Brushes.Transparent;
            SnapsToDevicePixels = true;
            UseLayoutRounding = true;

            scrollContainer = new ScrollViewer()
            {
                CanContentScroll = true,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            canvas = new GridCanvas()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top
            };

            scrollContainer.Content = canvas;

            this.Content = scrollContainer;
        }

        protected virtual void Hook()
        {
            Stage.JoinChanged += StageJoinChanged;
        }

        protected virtual void Unhook()
        {
            Stage.JoinChanged -= StageJoinChanged;
        }

        protected abstract ScreenCanvas CreateScreenCanvas(ScreenDocument screen);

        protected abstract void DestroyScreenCanvas(ScreenCanvas canvas);

        private void StageJoinChanged(Join obj)
        {
            if (!_freezeLayout)
            {
                LayoutScreens();
            }
        }

        private void StageLayoutControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetSize();
        }

        private void SetSize()
        {
            canvas.Width = Math.Max(_stageSize.Width, scrollContainer.ActualWidth);
            canvas.Height = Math.Max(_stageSize.Height, scrollContainer.ActualHeight);

            scrollContainer.HorizontalScrollBarVisibility = (scrollContainer.ActualWidth < _stageSize.Width) ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
            scrollContainer.VerticalScrollBarVisibility = (scrollContainer.ActualHeight < _stageSize.Height) ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
        }

        private void ResetScreens()
        {
            // recycle objects if we can
            var canvases = _screens.Values.ToArray();
            var screenDocuments = Stage.Screens.ToArray();

            _screens.Clear();

            if (canvases.Length >= screenDocuments.Length)
            {
                for (int i = 0; i < screenDocuments.Length; i++)
                {
                    canvases[i].Screen = screenDocuments[i];
                }

                for (int i = screenDocuments.Length; i < canvases.Length; i++)
                {
                    DestroyScreenCanvas(canvases[i]);
                }

                canvas.Children.RemoveRange(screenDocuments.Length, canvases.Length - screenDocuments.Length);
            }
            else
            {
                for (int i = 0; i < canvases.Length; i++)
                {
                    canvases[i].Screen = screenDocuments[i];
                }

                for (int i = canvases.Length; i < screenDocuments.Length; i++)
                {
                    var screen = CreateScreenCanvas(screenDocuments[i]);

                    canvas.Children.Add(screen);
                }
            }

            foreach (ScreenCanvas child in canvas.Children)
            {
                _screens[child.Screen.Name] = child;
            }

            LayoutScreens();
        }

        protected void LayoutScreens()
        {
            if (canvas.Children.Count == 0) return;

            var placeable = new HashSet<string>();
            var orphans = new List<string>();

            _screensPlaced.Clear();

            int minX = 0, minY = 0, maxX = 0, maxY = 0;

            var startScreen = _screens.Values.First();
            if (_screens.ContainsKey(Stage.StartScreen))
            {
                startScreen = _screens[Stage.StartScreen];
            }

            // lay the screens out like a deep graph traversal
            LayoutFromScreen(startScreen, MegaMan.Common.Geometry.Point.Empty);

            // any remaining disconnected screens
            foreach (var surface in _screens.Values.Where(s => !_screensPlaced.Contains(s.Screen.Name)))
            {
                LayoutFromScreen(surface, MegaMan.Common.Geometry.Point.Empty);
            }

            foreach (var surface in _screens.Values)
            {
                var location = GetCanvasLocation(surface);
                minX = Math.Min(minX, location.X);
                minY = Math.Min(minY, location.Y);
            }

            if (minX < 0 || minY < 0)
            {
                // now readjust to all positive locations
                foreach (var surface in _screens.Values)
                {
                    var location = GetCanvasLocation(surface);
                    location.Offset(-minX, -minY);

                    SetCanvasLocation(surface, location);
                }
            }

            foreach (var surface in _screens.Values)
            {
                var location = GetCanvasLocation(surface);
                var right = location.X + surface.Screen.PixelWidth;
                var bottom = location.Y + surface.Screen.PixelHeight;

                maxX = Math.Max(maxX, right);
                maxY = Math.Max(maxY, bottom);
            }

            _stageSize = new Size(maxX, maxY);

            SetSize();
        }

        private MegaMan.Common.Geometry.Point GetCanvasLocation(ScreenCanvas surface)
        {
            return new MegaMan.Common.Geometry.Point((int)surface.Margin.Left, (int)surface.Margin.Top);
        }

        private void SetCanvasLocation(ScreenCanvas surface, MegaMan.Common.Geometry.Point location)
        {
            surface.Margin = new Thickness(location.X, location.Y, 0, 0);
        }

        private void LayoutFromScreen(ScreenCanvas surface, MegaMan.Common.Geometry.Point location)
        {
            SetCanvasLocation(surface, location);

            _screensPlaced.Add(surface.Screen.Name);

            var myJoins = surface.Screen.Joins;
            var joinedScreens = _screens.Values.Where(s => s.Screen.Joins.Intersect(myJoins).Any());

            var placed = _screens.Values.Where(s => _screensPlaced.Contains(s.Screen.Name) && s != surface && !joinedScreens.Contains(s));
            var collision = SurfaceCollides(placed, surface);
            while (collision != Rectangle.Empty)
            {
                TryToFixCollision(surface, collision);
                collision = SurfaceCollides(placed, surface);
            }

            foreach (var join in Stage.Joins.Where(j => j.screenOne == surface.Screen.Name))
            {
                var nextScreen = _screens[join.screenTwo];
                if (_screensPlaced.Contains(nextScreen.Screen.Name)) continue;

                LayoutNextScreen(nextScreen, GetCanvasLocation(surface), join, true);
            }

            foreach (var join in Stage.Joins.Where(j => j.screenTwo == surface.Screen.Name))
            {
                var nextScreen = _screens[join.screenOne];
                if (_screensPlaced.Contains(nextScreen.Screen.Name)) continue;

                LayoutNextScreen(nextScreen, GetCanvasLocation(surface), join, false);
            }
        }

        private void LayoutNextScreen(ScreenCanvas surface, MegaMan.Common.Geometry.Point location, Join join, bool one)
        {
            int offsetX = location.X;
            int offsetY = location.Y;
            int mag = one ? 1 : -1;

            if (join.type == JoinType.Horizontal)
            {
                offsetX += (join.offsetOne - join.offsetTwo) * Stage.Tileset.TileSize * mag;
                offsetY += _screens[join.screenOne].Screen.PixelHeight * mag;
            }
            else
            {
                offsetX += _screens[join.screenOne].Screen.PixelWidth * mag;
                offsetY += (join.offsetOne - join.offsetTwo) * Stage.Tileset.TileSize * mag;
            }

            LayoutFromScreen(surface, new MegaMan.Common.Geometry.Point(offsetX, offsetY));
        }

        private Rectangle SurfaceCollides(IEnumerable<ScreenCanvas> placedAlready, ScreenCanvas next)
        {
            Rectangle collisions = Rectangle.Empty;
            var location = GetCanvasLocation(next);
            Rectangle nextRect = new Rectangle(location.X, location.Y, next.Screen.PixelWidth, next.Screen.PixelHeight);

            foreach (var surface in placedAlready)
            {
                var surfaceLocation = GetCanvasLocation(surface);
                var surfaceRect = new Rectangle(surfaceLocation.X, surfaceLocation.Y, surface.Screen.PixelWidth, surface.Screen.PixelHeight);
                Rectangle inter = Rectangle.Intersect(surfaceRect, nextRect);
                if (inter.Width > 0 && inter.Height > 0)
                {
                    if (collisions.Height > 0 && collisions.Width > 0) collisions = Rectangle.Union(collisions, inter);
                    else collisions = inter;
                }
            }
            return collisions;
        }

        private void TryToFixCollision(ScreenCanvas surface, Rectangle collision)
        {
            var location = GetCanvasLocation(surface);
            MegaMan.Common.Geometry.Point collCenter = location;
            collCenter.Offset(collision.Width / 2, collision.Height / 2);

            MegaMan.Common.Geometry.Point surfCenter = location;
            surfCenter.Offset((int)(surface.Width / 2), (int)(surface.Height / 2));

            int off_y = surfCenter.Y - collCenter.Y;
            int off_x = surfCenter.X - collCenter.X;
            if (Math.Abs(off_y) > Math.Abs(off_x))
            {
                SetCanvasLocation(surface,
                    new MegaMan.Common.Geometry.Point(location.X, location.Y + surface.Screen.Tileset.TileSize));
            }
            else
            {
                SetCanvasLocation(surface,
                    new MegaMan.Common.Geometry.Point(location.X + surface.Screen.Tileset.TileSize, location.Y)); 
            }
        }
    }
}
