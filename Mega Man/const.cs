using System.Windows.Forms;

namespace MegaMan.Engine
{
    public class Const
    {
        // ===================== ENGINE ===================
        /// <summary>
        /// Defines the enforced framerate for the game.
        /// </summary>
        public const int FPS = 60;

        /// <summary>
        /// Images are stupid, and require resolution even for pixel to pixel conversion.
        /// </summary>
        public const int Resolution = 72;

        public const float PixelEpsilon = 0.001f;

        // ==================== GAME =====================
        public const int PixelsAcross = 256;
        public const int PixelsDown = 224;

        /// <summary>
        /// Defines the speed of screen scrolling, in pixels per frame.
        /// </summary>
        public const float ScrollSpeed = 3.0f;

        /// <summary>
        /// How long the map hangs around after the player dies before resetting.
        /// </summary>
        public const int MapDeadFrames = 240;

        /// <summary>
        /// Defines how many pixels from the edge the player will be placed after a screen scroll.
        /// </summary>
        public const int PlayerScrollOffset = 4;

        /// <summary>
        /// Defines how close to the edge, in pixels, the player must be to trigger a scroll.
        /// Must be smaller than PlayerScrollOffset or else it will scroll back and forth forever.
        /// </summary>
        public const int PlayerScrollTrigger = 0;

        /// <summary>
        /// Terminal velocity for falling entities. Eventually this may be controlled in the
        /// entity definitions, but it's useful to set a global constraint so at least everyone agrees.
        /// </summary>
        public const float TerminalVel = 12;
    }

    public enum GameInput
    {
        Up,
        Down,
        Left,
        Right,
        Jump,
        Shoot,
        Start,
        Select,
        None
    }

    /// <summary>
    /// This class translates keyboard input keys into game input keys.
    /// Note that it still holds Keys enum values, not GameInput values.
    /// The keys are public so they can be changed by the config.
    /// It's kind of like an enum, but more useful.
    /// </summary>
    public class GameInputKeys : System.Collections.IEnumerable
    {
        public static Keys Right = Keys.Right;
        public static Keys Left = Keys.Left;
        public static Keys Up = Keys.Up;
        public static Keys Down = Keys.Down;
        public static Keys Jump = Keys.S;
        public static Keys Shoot = Keys.A;
        public static Keys Start = Keys.Enter;
        public static Keys Select = Keys.Shift;

        private static GameInputKeys instance;
        public static GameInputKeys Instance
        {
            get { return instance ?? (instance = new GameInputKeys()); }
        }

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            yield return Right;
            yield return Left;
            yield return Up;
            yield return Down;
            yield return Jump;
            yield return Shoot;
            yield return Start;
            yield return Select;
        }

        #endregion
    }
}