using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MegaMan.Editor.Tools
{
    public class CleaveToolCursor : IToolCursor
    {
        private ImageSource _image;

        public CleaveToolCursor()
        {
            _image = new BitmapImage(new Uri(@"pack://application:,,,/"
             + Assembly.GetExecutingAssembly().GetName().Name
             + ";component/"
             + "Resources/vsplit.png", UriKind.Absolute)); 
        }

        public ImageSource CursorImage
        {
            get { return _image; }
        }

        public double CursorWidth
        {
            get { return _image.Width; }
        }

        public double CursorHeight
        {
            get { return _image.Height; }
        }
    }
}
