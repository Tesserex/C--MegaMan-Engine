using System.Runtime.InteropServices;

namespace MegaMan.Engine.Forms
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
            var ret = GetAsyncKeyState((int)key);
            return (ret != 0);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if !DEBUG
            try
            {
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
#if !DEBUG
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an unhandled error. I'm sorry, but I have to close.\nDetails will be sent to the developer.", "C# MegaMan Engine", MessageBoxButtons.OK, MessageBoxIcon.Error);

                var reportCrash = new ReportCrash("tesserex@gmail.com") {
                    Silent = true,
                    DoctorDumpSettings = new DoctorDumpSettings() {
                        ApplicationID = new Guid("7c95b7dc-98c4-418c-ba92-096a986b56ee"),
                        SendAnonymousReportSilently = true
                    }
                };
                reportCrash.Send(ex);
            }
#endif
        }
    }
}
