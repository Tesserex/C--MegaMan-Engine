using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace MegaMan.Editor.Tools
{
    public class StandardToolCursor : IToolCursor
    {
        private Cursor cursor;

        public StandardToolCursor(string cursorResourceName)
        {
            var stream = Application.GetResourceStream(new Uri(Assembly.GetExecutingAssembly().GetName().Name + ";component/Resources/" + cursorResourceName, UriKind.Relative));
            cursor = new Cursor(stream.Stream);
        }

        public void ApplyCursorTo(FrameworkElement element)
        {
            element.Cursor = cursor;
        }

        public void Dispose()
        {
            cursor.Dispose();
        }
    }
}
