using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Editor.Controls
{
    public class TileScreenCanvas : ScreenCanvas
    {
        private IToolProvider _toolProvider;

        public TileScreenCanvas(IToolProvider toolProvider)
        {
            _toolProvider = toolProvider;
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (_toolProvider.Tool == null)
            {
                return;
            }

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.Click(this.Screen, new Common.Geometry.Point((int)mousePoint.X, (int)mousePoint.Y));
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_toolProvider.Tool == null)
            {
                return;
            }

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.Release(this.Screen);
        }

        protected override void OnMouseRightButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (_toolProvider.Tool == null)
            {
                return;
            }

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.RightClick(this.Screen, new Common.Geometry.Point((int)mousePoint.X, (int)mousePoint.Y));
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_toolProvider.Tool == null)
            {
                return;
            }

            var mousePoint = e.GetPosition(this);

            _toolProvider.Tool.Move(this.Screen, new Common.Geometry.Point((int)mousePoint.X, (int)mousePoint.Y));
        }
    }
}
