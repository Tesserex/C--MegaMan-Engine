﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Bll;
using System.Windows.Media;
using System.Windows.Documents;
using MegaMan.Editor.Bll.Algorithms;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for StageLayoutControl.xaml
    /// </summary>
    public abstract class StageControl : UserControl, IRequireCurrentStage
    {
        internal ScrollViewer scrollContainer;

        internal GridCanvas canvas;

        protected AdornerLayer adornerLayer;

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
                }

                ResetScreens();

                InvalidateVisual();
            }
        }

        public void SetStage(StageDocument stage)
        {
            Stage = stage;
        }

        public void UnsetStage()
        {
            Stage = null;
            _screens.Clear();
            canvas.Children.Clear();
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

            var adornerDecorator = new System.Windows.Documents.AdornerDecorator();
            adornerDecorator.Child = scrollContainer;

            this.adornerLayer = adornerDecorator.AdornerLayer;

            scrollContainer.Content = canvas;

            this.Content = adornerDecorator;
        }

        protected virtual void Hook()
        {
            Stage.JoinChanged += StageJoinChanged;
            Stage.ScreenResized += ScreenResized;
        }

        protected virtual void Unhook()
        {
            Stage.JoinChanged -= StageJoinChanged;
        }

        protected abstract ScreenCanvas CreateScreenCanvas(ScreenDocument screen);

        protected abstract void DestroyScreenCanvas(ScreenCanvas canvas);

        private void ScreenResized(ScreenDocument screen, int width, int height)
        {
            LayoutScreens();
        }

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

            var arranger = new ScreenLayoutArranger(this.Stage);
            arranger.Arrange();

            int maxX = 0, maxY = 0;

            foreach (var screenPointPair in arranger.ScreenPositions)
            {
                var surface = _screens[screenPointPair.Key];

                SetCanvasLocation(surface, new Common.Geometry.Point(screenPointPair.Value.X * surface.Screen.Tileset.TileSize, screenPointPair.Value.Y * surface.Screen.Tileset.TileSize));
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
    }
}