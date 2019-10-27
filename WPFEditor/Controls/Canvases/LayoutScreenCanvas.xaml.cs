using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Editor.Controls.Adorners;
using MegaMan.Editor.Tools;

namespace MegaMan.Editor.Controls
{
    public partial class LayoutScreenCanvas : ScreenCanvas
    {
        private ScreenResizeAdorner _adorner;
        private LayoutObjectsLayer _objectsLayer;

        public LayoutScreenCanvas(IToolProvider toolProvider)
            : base(toolProvider)
        {
            InitializeComponent();

            _objectsLayer = new LayoutObjectsLayer();
            Children.Insert(1, _objectsLayer);

            Loaded += AddAdorners;

            _tiles.RenderGrayscale();
        }

        private void AddAdorners(object sender, RoutedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            _adorner = new ScreenResizeAdorner(this, Screen);
            adornerLayer.Add(_adorner);
        }

        protected override void ScreenChanged()
        {
            base.ScreenChanged();
            _objectsLayer.Screen = Screen;
            if (_adorner != null)
                _adorner.Screen = Screen;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            _tiles.RenderColor();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            _tiles.RenderGrayscale();
        }

        private void CloneClicked(object sender, RoutedEventArgs e)
        {
            Screen.Clone();
        }

        private void DeleteClicked(object sender, RoutedEventArgs e)
        {
            Screen.Delete();
        }
    }
}
