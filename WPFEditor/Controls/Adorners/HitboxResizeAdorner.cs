using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MegaMan.Editor.Controls.ViewModels.Entities.Components;

namespace MegaMan.Editor.Controls.Adorners {
    public class HitboxResizeAdorner : Adorner
    {
        private HitboxEditorViewModel viewModel;

        // Resizing adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        private Thumb topLeft, topRight, bottomLeft, bottomRight;

        // To store and manage the adorner's visual children.
        private VisualCollection visualChildren;

        // Initialize the ResizingAdorner.
        public HitboxResizeAdorner(UIElement adornedElement, HitboxEditorViewModel viewModel)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);
            this.viewModel = viewModel;

            // Call a helper method to initialize the Thumbs
            // with a customized cursors.
            BuildAdornerCorner(ref topLeft, Cursors.SizeNWSE);
            BuildAdornerCorner(ref topRight, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomLeft, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomRight, Cursors.SizeNWSE);

            // Add handlers for resizing.
            bottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeft);
            bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);

            adornedElement.MouseEnter += adornedElement_MouseEnter;
            adornedElement.MouseLeave += adornedElement_MouseLeave;
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        protected override void OnMouseEnter(MouseEventArgs e) {
            base.OnMouseEnter(e);
            this.Visibility = System.Windows.Visibility.Visible;
            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e) {
            base.OnMouseLeave(e);
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        void adornedElement_MouseLeave(object sender, MouseEventArgs e) {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        void adornedElement_MouseEnter(object sender, MouseEventArgs e) {
            this.Visibility = System.Windows.Visibility.Visible;
            InvalidateVisual();
        }

        // Handler for resizing from the bottom-right.
        void HandleBottomRight(object sender, DragDeltaEventArgs args) {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            viewModel.ZoomWidth = (int)Math.Max(viewModel.ZoomWidth + args.HorizontalChange, hitThumb.DesiredSize.Width);
            viewModel.ZoomHeight = (int)Math.Max(args.VerticalChange + viewModel.ZoomHeight, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-right.
        void HandleTopRight(object sender, DragDeltaEventArgs args) {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            viewModel.ZoomWidth = (int)Math.Max(viewModel.ZoomWidth + args.HorizontalChange, hitThumb.DesiredSize.Width);

            var height_old = viewModel.ZoomHeight;
            var height_new = (int)Math.Max(viewModel.ZoomHeight - args.VerticalChange, hitThumb.DesiredSize.Height);
            var top_old = (int)Canvas.GetTop(adornedElement);
            viewModel.ZoomHeight = height_new;
            viewModel.ZoomTop = top_old - (height_new - height_old);
        }

        // Handler for resizing from the top-left.
        void HandleTopLeft(object sender, DragDeltaEventArgs args) {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.

            var width_old = viewModel.ZoomWidth;
            var width_new = (int)Math.Max(viewModel.ZoomWidth - args.HorizontalChange, hitThumb.DesiredSize.Width);
            var left_old = (int)Canvas.GetLeft(adornedElement);
            viewModel.ZoomWidth = width_new;
            viewModel.ZoomLeft = left_old - (width_new - width_old);

            var height_old = viewModel.ZoomHeight;
            var height_new = (int)Math.Max(viewModel.ZoomHeight - args.VerticalChange, hitThumb.DesiredSize.Height);
            var top_old = (int)Canvas.GetTop(adornedElement);
            viewModel.ZoomHeight = height_new;
            viewModel.ZoomTop = top_old - (height_new - height_old);
        }

        // Handler for resizing from the bottom-left.
        void HandleBottomLeft(object sender, DragDeltaEventArgs args) {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            viewModel.ZoomHeight = (int)Math.Max(args.VerticalChange + viewModel.ZoomHeight, hitThumb.DesiredSize.Height);

            var width_old = viewModel.ZoomWidth;
            var width_new = (int)Math.Max(viewModel.ZoomWidth - args.HorizontalChange, hitThumb.DesiredSize.Width);
            var left_old = (int)Canvas.GetLeft(adornedElement);
            viewModel.ZoomWidth = width_new;
            viewModel.ZoomLeft = left_old - (width_new - width_old);
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize) {
            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.  
            // These will be used to place the ResizingAdorner at the corners of the adorned element.  
            var desiredWidth = AdornedElement.DesiredSize.Width;
            var desiredHeight = AdornedElement.DesiredSize.Height;
            // adornerWidth & adornerHeight are used for placement as well.
            var adornerWidth = this.DesiredSize.Width;
            var adornerHeight = this.DesiredSize.Height;

            topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            bottomLeft.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));

            // Return the final size.
            return finalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor) {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics.
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 12;
            cornerThumb.Background = new SolidColorBrush(Colors.DarkCyan);

            visualChildren.Add(cornerThumb);
        }

        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }
}
