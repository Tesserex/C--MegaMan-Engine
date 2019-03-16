using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MegaMan.Editor.Controls.Converters
{
    public class DpiDecorator : Decorator
    {
        public DpiDecorator()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Loaded += (s, e) => {
                    Matrix m = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice;
                    ScaleTransform dpiTransform = new ScaleTransform(1 / m.M11, 1 / m.M22);
                    if (dpiTransform.CanFreeze)
                        dpiTransform.Freeze();
                    this.LayoutTransform = dpiTransform;
                };
            }
        }
    }
}
