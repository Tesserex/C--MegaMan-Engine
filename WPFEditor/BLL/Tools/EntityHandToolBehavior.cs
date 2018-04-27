using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MegaMan.Editor.Controls;
using MegaMan.Editor.Controls.ViewModels;
using FrameworkElement = System.Windows.FrameworkElement;
using Point = MegaMan.Common.Geometry.Point;

namespace MegaMan.Editor.Bll.Tools
{
    public class EntityHandToolBehavior : IEntityToolBehavior
    {
        private EntityPlacementControl _grabbedEntity;
        private Point _offset;
        private Point _dragStart;

        public int SnapX { get; set; }
        public int SnapY { get; set; }

        public EntityHandToolBehavior(int snapX, int snapY)
        {
            SnapX = snapX;
            SnapY = snapY;
        }

        public void Click(ScreenCanvas canvas, Point location)
        {
            var entityLayer = canvas.Children.OfType<EntityScreenLayer>().SingleOrDefault();
            if (entityLayer != null)
            {
                var over = Mouse.DirectlyOver;
                if (over is FrameworkElement)
                {
                    var elem = (FrameworkElement)over;
                    while (!(elem is EntityPlacementControl) && elem != null)
                    {
                        elem = elem.Parent as FrameworkElement;
                    }

                    if (elem != null)
                    {
                        var placementControls = entityLayer.Children.OfType<EntityPlacementControl>();
                        if (placementControls.Contains(elem))
                        {
                            _grabbedEntity = (EntityPlacementControl)elem;

                            var vm = (EntityPlacementControlViewModel)_grabbedEntity.DataContext;
                            _dragStart = new Point(vm.Placement.screenX, vm.Placement.screenY);
                            _offset = new Point(location.X - vm.Placement.screenX, location.Y - vm.Placement.screenY);

                            // capture mouse last, because it triggers the move event
                            _grabbedEntity.CaptureMouse();
                        }
                    }
                }
                
            }
        }

        public void Move(ScreenCanvas canvas, Point location)
        {
            if (this._grabbedEntity != null)
            {
                var vm = (EntityPlacementControlViewModel)_grabbedEntity.DataContext;
                var endX = location.X - _offset.X;
                var endY = location.Y - _offset.Y;

                endX = (endX / SnapX) * SnapX;
                endY = (endY / SnapY) * SnapY;

                var end = new Point(endX, endY);

                canvas.Screen.MoveEntity(vm.Placement, end);
            }
        }

        public void Release(ScreenCanvas canvas, Point location)
        {
            if (this._grabbedEntity != null)
            {
                var vm = (EntityPlacementControlViewModel)_grabbedEntity.DataContext;
                var endpoint = new Point(vm.Placement.screenX, vm.Placement.screenY);

                var action = new MoveEntityAction(vm.Placement, canvas.Screen, _dragStart, endpoint);
                canvas.Screen.Stage.PushHistoryAction(action);

                this._grabbedEntity.ReleaseMouseCapture();
                this._grabbedEntity = null;
            }
        }

        public void RightClick(ScreenCanvas canvas, Point location)
        {

        }

        public bool SuppressContextMenu { get { return false; } }
    }
}
