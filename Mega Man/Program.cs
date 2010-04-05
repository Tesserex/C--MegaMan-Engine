using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Mega_Man
{
    [StructLayout(LayoutKind.Sequential)]
    struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam, lParam;
        public uint time;
        public Point pt;
    }

    static class Program
    {
        public static Random rand = new Random(0);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PeekMessage(out MSG msg, IntPtr hwnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        public static bool AppIdle
        {
            get
            {
                MSG msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        public static bool KeyDown(Keys key)
        {
            short ret = GetAsyncKeyState((int)key);
            return (ret != 0);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //try
            //{
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            //}
            //catch (System.IO.FileNotFoundException e)
            //{
             //   MessageBox.Show("I'm sorry, I couldn't find a file you were looking for. Perhaps the file path is incorrect?\n\n" + e.Message + "\n\nI need to close now. Sorry I don't have any more information.", "C# MegaMan Engine", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show("There was an unhandled error. I'm sorry, but I have to close.\nPlease give the following information to the developer:\n\n" + e.Message + "\n" + e.StackTrace, "C# MegaMan Engine", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
        }
    }
}
