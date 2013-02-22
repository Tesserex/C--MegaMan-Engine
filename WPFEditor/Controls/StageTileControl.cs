using MegaMan.Editor.Bll;
using MegaMan.Editor.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;

namespace MegaMan.Editor.Controls
{
    public class StageTileControl : StageControl
    {
        private ToolCursorAdorner _cursorAdorner;

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

        private void ToolChanged(object sender, ToolChangedEventArgs e)
        {
            UpdateCursor();
        }

        public StageTileControl() : base()
        {
            this.Loaded += StageTileControl_Loaded;
        }

        private void StageTileControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _cursorAdorner = new ToolCursorAdorner(this.canvas, ToolProvider);

            this.adornerLayer.Add(_cursorAdorner);
        }

        protected override ScreenCanvas CreateScreenCanvas(ScreenDocument screen)
        {
            var canvas = new TileScreenCanvas(ToolProvider);
            canvas.Screen = screen;

            return canvas;
        }

        protected override void DestroyScreenCanvas(ScreenCanvas canvas)
        {
            
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            UpdateCursor();
        }

        private void UpdateCursor()
        {
            if (ToolProvider != null && ToolProvider.ToolCursor != null)
            {
                Cursor = Cursors.None;
                this.adornerLayer.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.adornerLayer.Visibility = System.Windows.Visibility.Hidden;
                Cursor = Cursors.Arrow;
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            this.adornerLayer.Visibility = System.Windows.Visibility.Hidden;
            Cursor = Cursors.Arrow;
        }
    }
}
