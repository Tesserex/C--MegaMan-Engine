using System;
using System.Windows.Forms;

namespace MegaMan.LevelEditor {
    static class Program {
        private static Timer animTimer, frameTimer;
        public static event Action AnimateTick, FrameTick;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            animTimer = new Timer {Interval = (int) (1000/Const.FPS)};
            animTimer.Tick += timer_Tick;

            frameTimer = new Timer { Interval = (int)(1000 / Const.FPS) };
            frameTimer.Tick += frame_tick;
            frameTimer.Start();

            Application.Run(new MainForm());
        }

        static void timer_Tick(object sender, EventArgs e)
        {
            if (AnimateTick != null) AnimateTick();
        }

        static void frame_tick(object sender, EventArgs e)
        {
            if (FrameTick != null) FrameTick();
        }

        public static bool Animated
        {
            get { return animTimer.Enabled; }
            set { animTimer.Enabled = value; }
        }
    }
}
