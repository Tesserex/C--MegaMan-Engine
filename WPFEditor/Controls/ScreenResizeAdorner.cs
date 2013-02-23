using MegaMan.Editor.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MegaMan.Editor.Controls
{
    public class ScreenResizeAdorner : Adorner
    {
        private ScreenDocument _screen;

        private int _originalWidthTiles, _originalHeightTiles;

        private double _widthChangePixels, _heightChangePixels;

        // Resizing adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        private Thumb top, right, left, bottom;

        // To store and manage the adorner's visual children.
        private VisualCollection visualChildren;

        // Initialize the ResizingAdorner.
        public ScreenResizeAdorner(UIElement adornedElement, ScreenDocument screen)
            : base(adornedElement)
        {
            _screen = screen;

            visualChildren = new VisualCollection(this);

            // Call a helper method to initialize the Thumbs
            // with a customized cursors.
            BuildAdornerCorner(ref top, Cursors.SizeNS);
            BuildAdornerCorner(ref right, Cursors.SizeWE);
            BuildAdornerCorner(ref left, Cursors.SizeWE);
            BuildAdornerCorner(ref bottom, Cursors.SizeNS);

            // Add handlers for resizing.
            left.DragDelta += new DragDeltaEventHandler(HandleLeft);
            bottom.DragDelta += new DragDeltaEventHandler(HandleBottom);
            top.DragDelta += new DragDeltaEventHandler(HandleTop);
            right.DragDelta += new DragDeltaEventHandler(HandleRight);

            right.DragStarted += DragStarted;

            adornedElement.MouseEnter += adornedElement_MouseEnter;
            adornedElement.MouseLeave += adornedElement_MouseLeave;
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.Visibility = System.Windows.Visibility.Visible;
            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        void adornedElement_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        void adornedElement_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            InvalidateVisual();
        }

        private void DragStarted(object sender, DragStartedEventArgs e)
        {
            _originalWidthTiles = _screen.Width;
            _originalHeightTiles = _screen.Height;

            _widthChangePixels = 0;
            _heightChangePixels = 0;
        }

        // Handler for resizing from the bottom-right.
        void HandleBottom(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the bottom-left.
        void HandleLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
        }

        // Handler for resizing from the top-right.
        void HandleRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            // adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);

            _widthChangePixels = args.HorizontalChange;

            var tileChange = (int)(_widthChangePixels / _screen.Tileset.TileSize);
            var newWidth = _screen.Width + tileChange;

            if (newWidth != _screen.Width)
            {
                _screen.Resize(newWidth, _screen.Height);
                InvalidateMeasure();
                InvalidateVisual();
            }
        }

        // Handler for resizing from the top-left.
        void HandleTop(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.  
            // These will be used to place the ResizingAdorner at the corners of the adorned element.  
            double desiredWidth = AdornedElement.RenderSize.Width;
            double desiredHeight = AdornedElement.RenderSize.Height;

            top.Arrange(new Rect(0, -desiredHeight / 2, desiredWidth, desiredHeight));
            right.Arrange(new Rect(desiredWidth / 2, 0, desiredWidth, desiredHeight));
            left.Arrange(new Rect(-desiredWidth / 2, 0, desiredWidth, desiredHeight));
            bottom.Arrange(new Rect(0, desiredHeight / 2, desiredWidth, desiredHeight));

            // Return the final size.
            return finalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics.
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 12;
            cornerThumb.Background = new SolidColorBrush(Colors.DarkCyan);

            visualChildren.Add(cornerThumb);
        }

        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height
        // need to be set first.  It also sets the maximum size of the adorned element.
        void EnforceSize(FrameworkElement adornedElement)
        {
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.RenderSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.RenderSize.Height;

            FrameworkElement parent = adornedElement.Parent as FrameworkElement;
            if (parent != null)
            {
                adornedElement.MaxHeight = parent.ActualHeight;
                adornedElement.MaxWidth = parent.ActualWidth;
            }
        }
        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
    }
}
