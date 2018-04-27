﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Algorithms;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Tools;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for StageLayoutControl.xaml
    /// </summary>
    public abstract class StageControl : UserControl
    {
        internal ScrollViewer scrollContainer;
        internal GridCanvas canvas;

        protected AdornerLayer adornerLayer;

        protected Dictionary<string, ScreenCanvas> _screens;
        private bool _freezeLayout;
        private bool _resetNeeded;
        private bool _layoutNeeded;

        private HashSet<string> _screensPlaced;
        private Size _stageSize;

        public double Zoom { get; private set; }

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

        private IToolProvider _toolProvider;

        public IToolProvider ToolProvider
        {
            get
            {
                return _toolProvider;
            }
            set
            {
                if (_toolProvider != null)
                {
                    _toolProvider.ToolChanged -= ToolChanged;
                }

                _toolProvider = value;

                if (_toolProvider != null)
                {
                    _toolProvider.ToolChanged += ToolChanged;
                }
            }
        }

        public StageControl()
        {
            InitializeComponent();

            _screens = new Dictionary<string, ScreenCanvas>();
            _screensPlaced = new HashSet<string>();
            Zoom = 1;

            this.SizeChanged += StageLayoutControl_SizeChanged;

            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);
            ViewModelMediator.Current.GetEvent<ZoomChangedEventArgs>().Subscribe(ZoomChanged);
        }

        public void InitializeComponent()
        {
            Background = Brushes.Transparent;
            SnapsToDevicePixels = true;
            UseLayoutRounding = true;

            scrollContainer = new ScrollViewer() {
                CanContentScroll = true,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            canvas = new GridCanvas() {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top
            };

            var adornerDecorator = new System.Windows.Documents.AdornerDecorator();
            adornerDecorator.Child = scrollContainer;

            this.adornerLayer = adornerDecorator.AdornerLayer;

            scrollContainer.Content = canvas;

            this.Content = adornerDecorator;
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            if (e.Stage != null)
                Stage = e.Stage;
            else
                UnsetStage();
        }

        private void ZoomChanged(object sender, ZoomChangedEventArgs e)
        {
            Zoom = e.Zoom;
            LayoutScreens();
        }

        public void UnsetStage()
        {
            Stage = null;
            _screens.Clear();
            canvas.Children.Clear();
        }

        protected virtual void Hook()
        {
            Stage.JoinChanged += StageJoinChanged;
            Stage.ScreenResized += ScreenResized;
            Stage.ScreenAdded += ScreenAddedOrRemoved;
            Stage.ScreenRemoved += ScreenAddedOrRemoved;
        }

        protected virtual void Unhook()
        {
            Stage.JoinChanged -= StageJoinChanged;
            Stage.ScreenResized -= ScreenResized;
            Stage.ScreenAdded -= ScreenAddedOrRemoved;
            Stage.ScreenRemoved -= ScreenAddedOrRemoved;
        }

        protected abstract ScreenCanvas CreateScreenCanvas(ScreenDocument screen);

        protected void FreezeLayout()
        {
            _freezeLayout = true;
        }

        protected void UnfreezeLayout()
        {
            _freezeLayout = false;

            if (_resetNeeded)
            {
                ResetScreens();
            }
            else if (_layoutNeeded)
            {
                LayoutScreens();
            }
        }

        private void ToolChanged(object sender, ToolChangedEventArgs e)
        {
            UpdateCursor();
        }

        private void ScreenResized(ScreenDocument screen, int width, int height)
        {
            LayoutScreens();
        }

        private void ScreenAddedOrRemoved(ScreenDocument obj)
        {
            ResetScreens();
        }

        private void StageJoinChanged(Join obj)
        {
            LayoutScreens();
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
            if (_freezeLayout)
            {
                _resetNeeded = true;
                return;
            }

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
                    canvases[i].Destroy();
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

            _resetNeeded = false;
        }

        protected void LayoutScreens()
        {
            if (_freezeLayout)
            {
                _layoutNeeded = true;
                return;
            }

            if (canvas.Children.Count == 0) return;

            var arranger = new ScreenLayoutArranger(this.Stage);
            arranger.Arrange();

            int maxX = 0, maxY = 0;

            foreach (var screenPointPair in arranger.ScreenPositions)
            {
                var surface = _screens[screenPointPair.Key];

                var cx = (int)(screenPointPair.Value.X * surface.Screen.Tileset.TileSize * Zoom);
                var cy = (int)(screenPointPair.Value.Y * surface.Screen.Tileset.TileSize * Zoom);

                SetCanvasLocation(surface, new Common.Geometry.Point(cx, cy));
            }

            foreach (var surface in _screens.Values)
            {
                var location = GetCanvasLocation(surface);
                var right = (int)(location.X + surface.Screen.PixelWidth * Zoom);
                var bottom = (int)(location.Y + surface.Screen.PixelHeight * Zoom);

                maxX = Math.Max(maxX, right);
                maxY = Math.Max(maxY, bottom);
            }

            _stageSize = new Size(maxX, maxY);

            SetSize();

            _layoutNeeded = false;
        }

        private MegaMan.Common.Geometry.Point GetCanvasLocation(ScreenCanvas surface)
        {
            return new MegaMan.Common.Geometry.Point((int)surface.Margin.Left, (int)surface.Margin.Top);
        }

        private void SetCanvasLocation(ScreenCanvas surface, MegaMan.Common.Geometry.Point location)
        {
            surface.Margin = new Thickness(location.X, location.Y, 0, 0);
        }

        private IToolCursor _currentCursor;
        private void UpdateCursor()
        {
            if (_currentCursor != null)
            {
                _currentCursor.Dispose();
            }

            if (ToolProvider != null && ToolProvider.ToolCursor != null)
            {
                _currentCursor = ToolProvider.ToolCursor;
                ToolProvider.ToolCursor.ApplyCursorTo(this.scrollContainer);
            }
            else
            {
                this.Cursor = Cursors.Arrow;
                this.scrollContainer.Cursor = this.Cursor;
            }
        }
    }
}
